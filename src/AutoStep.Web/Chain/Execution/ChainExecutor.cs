using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AutoStep.Web.Chain.Execution
{
    /// <summary>
    /// Executes an element chain, with the necessary retry and error-handling behaviour.
    /// </summary>
    internal class ChainExecutor : IElementChainExecutor
    {
        private readonly IBrowser browser;
        private readonly ILogger logger;
        private readonly IChainDescriber describer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainExecutor"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="describer">A describer.</param>
        public ChainExecutor(IBrowser browser, ILogger<ChainExecutor> logger, IChainDescriber describer)
        {
            this.browser = browser;
            this.logger = logger;
            this.describer = describer;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Need to capture any and all exceptions that might arise from a chain. Actual exception is thrown via the ExceptionDispatchInfo.")]
        public async ValueTask<IReadOnlyList<IWebElement>> ExecuteAsync(IElementChain chain, CancellationToken cancellationToken)
        {
            var options = chain.Options;
            IReadOnlyList<IWebElement> results = Array.Empty<IWebElement>();
            Exception? lastException = null;
            var succeeded = false;
            var attemptCount = 0;

            if (chain.LeafNode is null)
            {
                // No point doing anything, the chain is empty.
                return results;
            }

            // First, build our list of execution nodes (in the reverse order of the chain's PreviousNode relationship).
            var entryPoint = chain.CreateExecutionEntryNode();

            // Get the last node with a cache (we'll try to start from this point).
            var lastWithCache = GetLastNodeWithCache(entryPoint);

            var startTime = DateTime.UtcNow;
            var timeoutSpan = TimeSpan.FromMilliseconds(options.TotalRetryTimeoutMs);

            do
            {
                try
                {
                    if (lastException is object)
                    {
                        if (logger.IsEnabled(LogLevel.Debug))
                        {
                            // The last attempt failed.
                            logger.LogDebug(ChainExecutorMessages.ElementChainAttemptFailure, attemptCount);
                            logger.LogDebug(describer.DescribeExecution(entryPoint, false));
                            logger.LogDebug(ChainExecutorMessages.WillRetry, options.RetryDelayMs);
                        }

                        // Give it a moment (the configured amount).
                        await Task.Delay(options.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                    }

                    attemptCount++;

                    if (await browser.WaitForPageReady(cancellationToken))
                    {
                        // First, start from the one that has enumerated elements already.
                        if (lastWithCache is object)
                        {
                            results = await Attempt(lastWithCache, browser, useElementsAtStart: true, cancellationToken);
                        }
                        else
                        {
                            results = await Attempt(entryPoint, browser, useElementsAtStart: false, cancellationToken);
                        }

                        // Store the final set of evaluated elements against the last node.
                        entryPoint.Last.CachedElements = results;

                        succeeded = true;
                        lastException = null;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    // If cancellation is thrown, stop immediately.
                    lastException = ex;
                    break;
                }
                catch (Exception ex)
                {
                    // When an error occurs, we want to know whether or not to bail.
                    // Definitely going to ignore the node that already has elements, that's not going to work for me.
                    lastWithCache = null;
                    lastException = ex;
                }
            }
            while (!succeeded && DateTime.UtcNow - startTime < timeoutSpan);

            if (!succeeded)
            {
                // Dump the failing node to a log, or some sort of context, and as much associated data as we can find.
                // Track the elements found at each node, including the start point, grouped by associated method info.
                if (lastException is OperationCanceledException)
                {
                    logger.LogWarning(ChainExecutorMessages.ElementChainCancellation);
                }
                else
                {
                    logger.LogError(ChainExecutorMessages.ElementChainError);

                    logger.LogError(describer.DescribeExecution(entryPoint, true));
                }

                if (lastException is object)
                {
                    // Throw the inner exception (but throw it from the original location).
                    ExceptionDispatchInfo.Capture(lastException).Throw();
                }
            }
            else
            {
                // At debug level, log successful chains as well.
                logger.LogDebug(ChainExecutorMessages.ElementChainSuccess);

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug(describer.DescribeExecution(entryPoint, true));
                }
            }

            return results;
        }

        private ExecutionNode? GetLastNodeWithCache(ExecutionNode entryPoint)
        {
            ExecutionNode? withCache = null;
            ExecutionNode? current = entryPoint;

            while (current is object)
            {
                if (current.CachedElements is object)
                {
                    withCache = current;
                }

                current = current.Next;
            }

            return withCache;
        }

        private async ValueTask<IReadOnlyList<IWebElement>> Attempt(ExecutionNode startPoint, IBrowser browser, bool useElementsAtStart, CancellationToken cancellationToken)
        {
            IReadOnlyList<IWebElement> elements;

            ExecutionNode? activeNode = startPoint;

            if (useElementsAtStart && startPoint.CachedElements is object)
            {
                elements = startPoint.CachedElements;

                // Start after this one.
                activeNode = activeNode.Next;
            }
            else
            {
                elements = Array.Empty<IWebElement>();
            }

            return await ExecuteNode(activeNode, elements, browser, cancellationToken);
        }

        private async ValueTask<IReadOnlyList<IWebElement>> ExecuteNode(ExecutionNode? activeNode, IReadOnlyList<IWebElement> elements, IBrowser browser, CancellationToken cancellationToken)
        {
            // We now have our starting point.
            // Go from there.
            while (activeNode is object)
            {
                activeNode.InputElements = elements;

                await activeNode.EnterNode(elements, browser, cancellationToken);

                // Process the children as well.
                foreach (var childNode in activeNode.ChildNodes)
                {
                    // All child nodes receive the same input elements.
                    await ExecuteNode(childNode, elements, browser, cancellationToken);
                }

                elements = activeNode.OutputElements = await activeNode.ExitNode(elements, browser, cancellationToken);

                activeNode = activeNode.Next;
            }

            return elements;
        }
    }
}
