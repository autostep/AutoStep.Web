using System.Collections.Generic;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Declaration
{
    /// <summary>
    /// Represents a declared node in an execution chain (which might then be executed multiple times).
    /// A declaration node is part of a singly-linked list that makes up the chain.
    /// </summary>
    public abstract class DeclarationNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeclarationNode"/> class.
        /// </summary>
        /// <param name="previousNode">The previous node in the chain. If null, this is the first node in the chain.</param>
        /// <param name="descriptor">A piece of descriptive text for the node, similar to a method name, used when reporting chain diagnostics.</param>
        /// <param name="modifiesSet">Indicates whether or not, when this node is executed, it modifies the set of elements.</param>
        /// <param name="executionContext">An optional execution context for the caller (used to improve descriptive output).</param>
        protected DeclarationNode(
            DeclarationNode? previousNode,
            string descriptor,
            bool modifiesSet,
            TestExecutionContext? executionContext)
        {
            PreviousNode = previousNode;
            Descriptor = descriptor;
            ModifiesSet = modifiesSet;
            ExecutionContext = executionContext;
        }

        /// <summary>
        /// Gets a description of the node.
        /// </summary>
        public string Descriptor { get; }

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
        /// Gets an optional execution context for the caller (used to improve descriptive output).
        /// </summary>
        public TestExecutionContext? ExecutionContext { get; }

        /// <summary>
        /// Gets or sets the list of elements cached against this declaration after the last chain execution.
        /// </summary>
        /// <remarks>
        /// This is the only mutable part of a declaration node; the outcome of a given execution can be cached against the last node
        /// in the chain, so re-executing the chain starts from this point.
        /// </remarks>
        internal IReadOnlyList<IWebElement>? CachedElements { get; set; }

        /// <summary>
        /// Creates an execution node from this declaration node.
        /// </summary>
        /// <returns>A new execution node.</returns>
        public abstract ExecutionNode CreateExecutionNode();
    }
}
