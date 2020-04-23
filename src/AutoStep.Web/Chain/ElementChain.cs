using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Provides the functionality to build a chain of element operations. Each instance of this object represents the
    /// end of the chain.
    /// </summary>
    public class ElementChain : IElementChain
    {
        public ElementChain(ElementChainOptions options)
            : this(null, options)
        {
        }

        public ElementChain(TestExecutionContext? executionContext, ElementChainOptions options)
            : this(null, executionContext, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementChain"/> class.
        /// </summary>
        /// <param name="node">The node at the end of the chain.</param>
        /// <param name="executionContext">The current AutoStep execution context (optional).</param>
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
            var newNode = new DeclarationNode(LeafNode, descriptor, callback, ActiveExecutionContext, true);

            return AddNode(newNode);
        }

        /// <inheritdoc/>
        public IElementChain AddNode(string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask> callback)
        {
            var newNode = new DeclarationNode(
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

        private IElementChain AddNode(DeclarationNode newNode)
        {
            return new ElementChain(newNode, ActiveExecutionContext, Options);
        }
    }
}
