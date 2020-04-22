using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AutoStep.Web.ElementChain
{
    internal class ElementChainExecutionStage : IDescribable
    {
        public ElementChainExecutionStage(ElementChainNode node)
        {
            Node = node;
        }

        public ElementChainNode Node { get; }

        public Exception? Error { get; set; }

        public IReadOnlyList<IWebElement>? InputElements { get; set; }

        public IReadOnlyList<IWebElement>? OutputElements { get; set; }

        public IReadOnlyList<IWebElement>? CachedElements
        { 
            get => Node.NodeEnumeratedElements;
            set => Node.NodeEnumeratedElements = value;
        }

        public MethodContext MethodContext => Node.MethodContext;

        public string Descriptor => Node.Descriptor;

        public bool WasExecuted => InputElements is object;

        public bool ModifiesSet => Node.ModifiesSet;

        public async ValueTask<IReadOnlyList<IWebElement>> Invoke(IReadOnlyList<IWebElement> previousElements, CancellationToken cancellationToken)
        {
            try
            {
                InputElements = previousElements;
                OutputElements = await Node.Callback(previousElements, cancellationToken);
                return OutputElements;
            }
            catch (Exception ex)
            {
                Error = ex;
                throw;
            }
        }
    }

    internal class ElementChainEvaluator
    {
        private ElementChainContext context;

        public ElementChainEvaluator(ElementChainContext context)
        {
            this.context = context;
        }

        public async ValueTask<IReadOnlyList<IWebElement>> Evaluate(ElementChainNode startNode, CancellationToken cancellationToken)
        {
            // Ok. First, find the last concrete node in the chain (gives us the set of starter elements, if any).            
            ElementChainNode? first = startNode;
            LinkedListNode<ElementChainExecutionStage>? firstWithElements = null;
            var executionSet = new LinkedList<ElementChainExecutionStage>();

            do
            {
                var addedNode = executionSet.AddFirst(new ElementChainExecutionStage(first));

                if (firstWithElements is null && first?.NodeEnumeratedElements is object)
                {
                    firstWithElements = addedNode;
                }

                first = first!.ChildNode;

            } while (first is object);

            IReadOnlyList<IWebElement> results = Array.Empty<IWebElement>();

            // Don't event bother if the set is empty.
            if (executionSet.First is null)
            {
                return results;
            }

            var succeeded = false;
            var attemptCount = 0;
            Exception? lastException = null;

            do
            {
                if (lastException is object)
                {
                    // Log it.
                    context.Logger.LogWarning("Element Chain Failure on attempt {0}", attemptCount);
                    context.Logger.LogWarning(context.Describer.DescribeExecution(executionSet, false));
                    context.Logger.LogWarning("Will retry in {0} ms.", context.Options.RetryDelayMs);

                    // Give it a moment.
                    await Task.Delay(context.Options.RetryDelayMs);
                }

                attemptCount++;

                try
                {
                    if (await context.Browser.WaitForPageReady(cancellationToken))
                    {
                        // First, start from the one that has enumerated elements already.
                        if (firstWithElements is object)
                        {
                            results = await Attempt(firstWithElements, useElementsAtStart: true, cancellationToken);
                        }
                        else
                        {
                            results = await Attempt(executionSet.First, false, cancellationToken);
                        }

                        // Store the final set of evaluated elements against the last node.
                        executionSet.Last!.Value.CachedElements = results;

                        succeeded = true;
                        lastException = null;
                    }
                }
                catch (Exception ex)
                {
                    // When an error occurs, we want to know whether or not to bail.
                    // Definitely going to ignore the node that already has elements, that's not going to work for me.
                    firstWithElements = null;
                    lastException = ex;
                }

            } while (!succeeded && attemptCount <= 10);
                        
            if (!succeeded)
            {
                // TODO:
                // Dump the failing node to a log, or some sort of context, and as much associated data as we can find.
                // Track the elements found at each node, including the start point, grouped by associated method info.

                // If we properly failed, capture the full element details.
                context.Logger.LogError("Element Chain Error:");
                context.Logger.LogError(context.Describer.DescribeExecution(executionSet, true));

                if (lastException is object)
                {
                    // Throw the inner exception (but throw it from the original location).
                    ExceptionDispatchInfo.Capture(lastException).Throw();
                }
            }
            else if (context.Logger.IsEnabled(LogLevel.Debug))
            {
                // At debug level, log successful chains as well.
                context.Logger.LogDebug("Element Chain Success:");
                context.Logger.LogDebug(context.Describer.DescribeExecution(executionSet, true));
            }

            return results;
        }

        private async ValueTask<IReadOnlyList<IWebElement>> Attempt(LinkedListNode<ElementChainExecutionStage> startPoint, bool useElementsAtStart, CancellationToken cancellationToken)
        {
            IReadOnlyList<IWebElement> elements;

            var activeNode = startPoint;

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
                elements = await activeNode.Value.Invoke(elements, cancellationToken);

                activeNode = activeNode.Next;
            }

            return elements;
        }
    }
}
