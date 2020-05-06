using System.Collections.Generic;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Declaration
{
    public abstract class DeclarationNode
    {
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
        /// Gets or sets the list of elements cached against this declaration after the last chain evaluation.
        /// </summary>
        internal IReadOnlyList<IWebElement>? CachedElements { get; set; }

        public abstract ExecutionNode CreateExecutionNode();
    }
}
