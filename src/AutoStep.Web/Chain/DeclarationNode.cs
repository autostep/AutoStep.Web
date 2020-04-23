using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Represents a declared node of an execution chain.
    /// </summary>
    public class DeclarationNode : IDescribable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeclarationNode"/> class.
        /// </summary>
        /// <param name="descriptor">A descriptor for the node.</param>
        /// <param name="callback">The callback to invoke when the node is executed.</param>
        /// <param name="executionContext">An optional execution context (improves description).</param>
        /// <param name="modifiesSet">Indicates whether this node can modify the set of elements passed to it.</param>
        public DeclarationNode(
            string descriptor,
            Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback,
            TestExecutionContext? executionContext,
            bool modifiesSet)
        {
            Callback = callback;
            ExecutionContext = executionContext;
            ModifiesSet = modifiesSet;
            Descriptor = descriptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeclarationNode"/> class.
        /// </summary>
        /// <param name="previousNode">The previous node in the chain.</param>
        /// <param name="descriptor">A descriptor for the node.</param>
        /// <param name="callback">The callback to invoke when the node is executed.</param>
        /// <param name="executionContext">An optional execution context (improves description).</param>
        /// <param name="modifiesSet">Indicates whether this node can modify the set of elements passed to it.</param>
        public DeclarationNode(
            DeclarationNode? previousNode,
            string descriptor,
            Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback,
            TestExecutionContext? executionContext,
            bool modifiesSet)
            : this(descriptor, callback, executionContext, modifiesSet)
        {
            PreviousNode = previousNode;
        }

        /// <summary>
        /// Gets a reference to the previous node in the chain.
        /// When this value is null, this node is the first node in the chain.
        /// </summary>
        public DeclarationNode? PreviousNode { get; }

        /// <summary>
        /// Gets a value indicating whether this node can modify the set of elements.
        /// </summary>
        public bool ModifiesSet { get; }

        /// <summary>
        /// Gets a description of the node.
        /// </summary>
        public string Descriptor { get; }

        /// <summary>
        /// Gets or sets the list of elements cached against this declaration after the last chain evaluation.
        /// </summary>
        internal IReadOnlyList<IWebElement>? CachedElements { get; set; }

        /// <summary>
        /// Gets the callback to invoke that executes this node.
        /// </summary>
        public Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> Callback { get; }

        /// <summary>
        /// Gets an optional execution context for the caller (used to improve descriptive output).
        /// </summary>
        public TestExecutionContext? ExecutionContext { get; }
    }
}
