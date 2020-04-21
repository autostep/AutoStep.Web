using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web.ElementChain
{
    internal class ElementChainEvaluator
    {
        private ElementChainContext context;

        public ElementChainEvaluator(ElementChainContext context)
        {
            this.context = context;
        }

        public async ValueTask<IEnumerable<IWebElement>> Evaluate(ElementChainNode startNode, CancellationToken cancellationToken)
        {
            // Ok. First, find the last concrete node in the chain (gives us the set of starter elements, if any).            
            ElementChainNode? first = startNode;
            LinkedListNode<ElementChainNode>? firstWithElements = null;
            var executionSet = new LinkedList<ElementChainNode>();

            do
            {
                var addedNode = executionSet.AddFirst(first);

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
            ElementQueryableEvaluationException? lastException = null;

            while (!succeeded && attemptCount < 10)
            {
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
                        executionSet.Last!.Value.NodeEnumeratedElements = results;

                        succeeded = true;
                    }
                }
                catch (ElementQueryableEvaluationException ex)
                {
                    // When an error occurs, we want to know whether or not to bail.
                    // Definitely going to ignore the node that already has elements, that's not going to work for me.
                    firstWithElements = null;

                    // Log it.
                    context.Logger.LogWarning(ex, "Error on Elements Query: {0}", ex.Message);

                    lastException = ex;

                    // Give it a moment.
                    await Task.Delay(context.Options.RetryDelayMs);
                }
            }

            if (!succeeded)
            {
                // TODO:
                // Dump the failing node to a log, or some sort of context, and as much associated data as we can find.
                // Track the elements found at each node, including the start point, grouped by associated method info.

                // Throw the inner exception.
                throw lastException!.InnerException!;
            }

            return results;
        }

        private async ValueTask<IReadOnlyList<IWebElement>> Attempt(LinkedListNode<ElementChainNode> startPoint, bool useElementsAtStart, CancellationToken cancellationToken)
        {
            IReadOnlyList<IWebElement> elements;

            var activeNode = startPoint;

            if (useElementsAtStart && startPoint.Value.NodeEnumeratedElements is object)
            {
                elements = startPoint.Value.NodeEnumeratedElements;

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
                elements = await InvokeNode(activeNode, elements, cancellationToken);

                activeNode = activeNode.Next;
            }

            return elements;
        }

        private async ValueTask<IReadOnlyList<IWebElement>> InvokeNode(LinkedListNode<ElementChainNode> node, IReadOnlyList<IWebElement> previousElements, CancellationToken cancellationToken)
        {
            try
            {
                return await node.Value.Callback(previousElements, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ElementQueryableEvaluationException(node, ex);
            }
        }

        private class ElementQueryableEvaluationException : Exception
        {
            internal ElementQueryableEvaluationException(LinkedListNode<ElementChainNode> failingNode, Exception innerException)
                : base("Elements Evaluation Exception.", innerException)
            {
                FailingNode = failingNode;
            }

            public LinkedListNode<ElementChainNode> FailingNode { get; }
        }
    }
}
