using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AutoStep.Web.Chain
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
            LinkedListNode<ExecutionNode>? firstWithCache;
            var succeeded = false;
            var attemptCount = 0;

            if (chain.LeafNode is null)
            {
                // No point doing anything, the chain is empty.
                return results;
            }

            // First, build our list of execution nodes (in the reverse order of the chain's PreviousNode relationship).
            var executionSet = BuildExecutionSet(chain.LeafNode, out firstWithCache);

            do
            {
                try
                {
                    if (lastException is object)
                    {
                        // The last attempt failed.
                        logger.LogWarning(ChainExecutorMessages.ElementChainAttemptFailure, attemptCount);
                        logger.LogWarning(describer.DescribeExecution(executionSet, false));
                        logger.LogWarning(ChainExecutorMessages.WillRetry, options.RetryDelayMs);

                        // Give it a moment (the configured amount).
                        await Task.Delay(options.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                    }

                    attemptCount++;

                    if (await browser.WaitForPageReady(cancellationToken))
                    {
                        // First, start from the one that has enumerated elements already.
                        if (firstWithCache is object)
                        {
                            results = await Attempt(firstWithCache, browser, useElementsAtStart: true, cancellationToken);
                        }
                        else
                        {
                            results = await Attempt(executionSet.First!, browser, useElementsAtStart: false, cancellationToken);
                        }

                        // Store the final set of evaluated elements against the last node.
                        executionSet.Last!.Value.CachedElements = results;

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
                    firstWithCache = null;
                    lastException = ex;
                }
            }
            while (!succeeded && attemptCount <= 10);

            if (!succeeded)
            {
                // TODO:
                // Dump the failing node to a log, or some sort of context, and as much associated data as we can find.
                // Track the elements found at each node, including the start point, grouped by associated method info.
                if (lastException is OperationCanceledException)
                {
                    logger.LogWarning(ChainExecutorMessages.ElementChainCancellation);
                }
                else
                {
                    logger.LogError(ChainExecutorMessages.ElementChainError);

                    logger.LogError(describer.DescribeExecution(executionSet, true));
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
                    logger.LogDebug(describer.DescribeExecution(executionSet, true));
                }
            }

            return results;
        }

        private static LinkedList<ExecutionNode> BuildExecutionSet(DeclarationNode start, out LinkedListNode<ExecutionNode>? firstWithCache)
        {
            var executionSet = new LinkedList<ExecutionNode>();
            DeclarationNode? current = start;

            firstWithCache = null;

            // First node executes first.
            do
            {
                var addedNode = executionSet.AddFirst(new ExecutionNode(current));

                // If we have a node that has cached elements, we will try to use it (but only on the first attempt).
                if (firstWithCache is null && current?.CachedElements is object)
                {
                    firstWithCache = addedNode;
                }

                current = current!.PreviousNode;
            }
            while (current is object);

            return executionSet;
        }

        private async ValueTask<IReadOnlyList<IWebElement>> Attempt(LinkedListNode<ExecutionNode> startPoint, IBrowser browser, bool useElementsAtStart, CancellationToken cancellationToken)
        {
            IReadOnlyList<IWebElement> elements;

            LinkedListNode<ExecutionNode>? activeNode = startPoint;

            if (useElementsAtStart && startPoint.Value.CachedElements is object)
            {
                elements = startPoint.Value.CachedElements;

                // Start after this one.
                activeNode = activeNode.Next;
            }
            else
            {
                elements = Array.Empty<IWebElement>();
            }

            // We now have our starting point.
            // Go from there.
            while (activeNode is object)
            {
                elements = await activeNode.Value.Invoke(elements, browser, cancellationToken);

                activeNode = activeNode.Next;
            }

            return elements;
        }
    }
}
