using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Declaration;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Provides the functionality to build a chain of element operations. Each instance of this object represents the
    /// end of the chain.
    /// </summary>
    public class ElementChain : IElementChain
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementChain"/> class.
        /// </summary>
        /// <param name="options">The set of chain options.</param>
        public ElementChain(ElementChainOptions options)
            : this(null, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementChain"/> class.
        /// </summary>
        /// <param name="executionContext">The current AutoStep execution context (optional).</param>
        /// <param name="options">The set of chain options.</param>
        public ElementChain(TestExecutionContext? executionContext, ElementChainOptions options)
            : this(null, executionContext, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementChain"/> class.
        /// </summary>
        /// <param name="node">The node at the end of the chain.</param>
        /// <param name="executionContext">The current AutoStep execution context (optional).</param>
        /// <param name="options">The set of chain options.</param>
        public ElementChain(DeclarationNode? node, TestExecutionContext? executionContext, ElementChainOptions options)
        {
            LeafNode = node;
            ActiveExecutionContext = executionContext;
            Options = options;
        }

        /// <summary>
        /// Gets the last node in the chain.
        /// </summary>
        public DeclarationNode? LeafNode { get; }

        /// <summary>
        /// Gets the active execution context for this point in the chain.
        /// </summary>
        public TestExecutionContext? ActiveExecutionContext { get; }

        /// <summary>
        /// Gets a value indicating whether there have been any chain operations prior to this one.
        /// </summary>
        public bool AnyPreviousNodes => LeafNode != null;

        /// <summary>
        /// Gets the chain options.
        /// </summary>
        public ElementChainOptions Options { get; }

        /// <inheritdoc/>
        public IElementChain AddNode(string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback)
        {
            if (string.IsNullOrEmpty(descriptor))
            {
                throw new ArgumentException(ElementChainMessages.BlankStringParameter, nameof(descriptor));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var newNode = new SingleNode(LeafNode, descriptor, callback, ActiveExecutionContext, true);

            return AddNode(newNode);
        }

        /// <inheritdoc/>
        public IElementChain AddNode(string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask> callback)
        {
            if (string.IsNullOrEmpty(descriptor))
            {
                throw new ArgumentException(ElementChainMessages.BlankStringParameter, nameof(descriptor));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var newNode = new SingleNode(
                LeafNode,
                descriptor,
                async (webElements, browser, cancelToken) =>
                {
                    // Invoke callback and return input elements.
                    await callback(webElements, browser, cancelToken);
                    return webElements;
                },
                ActiveExecutionContext,
                false);

            return AddNode(newNode);
        }

        public IElementChain AddGroupingNode(string descriptor, Func<IEnumerable<IReadOnlyList<IWebElement>>, IReadOnlyList<IWebElement>> reducer, params Func<IElementChain, IElementChain>[] chainBuilders)
        {
            if (string.IsNullOrEmpty(descriptor))
            {
                throw new ArgumentException(ElementChainMessages.BlankStringParameter, nameof(descriptor));
            }

            if (reducer is null)
            {
                throw new ArgumentNullException(nameof(reducer));
            }

            var chains = new List<IElementChain>();

            foreach (var builder in chainBuilders)
            {
                chains.Add(builder(new ElementChain(ActiveExecutionContext, Options)));
            }

            var newNode = new GroupingNode(
                LeafNode,
                descriptor,
                ActiveExecutionContext,
                reducer,
                chains);

            return AddNode(newNode);
        }

        private IElementChain AddNode(DeclarationNode newNode)
        {
            return new ElementChain(newNode, ActiveExecutionContext, Options);
        }

        public ExecutionNode? CreateExecutionEntryNode()
        {
            ExecutionNode? firstExecutionNode = null;
            DeclarationNode? current = LeafNode;

            // First node executes first.
            while (current is object)
            {
                var newNode = current.CreateExecutionNode();

                newNode.Next = firstExecutionNode;

                if (firstExecutionNode is object)
                {
                    firstExecutionNode.Previous = newNode;
                }

                firstExecutionNode = newNode;

                current = current!.PreviousNode;
            }

            return firstExecutionNode;
        }
    }
}
