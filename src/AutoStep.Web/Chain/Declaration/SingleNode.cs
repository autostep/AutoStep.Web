using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Declaration
{

    /// <summary>
    /// Represents a declared node of an execution chain.
    /// </summary>
    public class SingleNode : DeclarationNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleNode"/> class.
        /// </summary>
        /// <param name="descriptor">A descriptor for the node.</param>
        /// <param name="callback">The callback to invoke when the node is executed.</param>
        /// <param name="executionContext">An optional execution context (improves description).</param>
        /// <param name="modifiesSet">Indicates whether this node can modify the set of elements passed to it.</param>
        public SingleNode(
            string descriptor,
            Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback,
            TestExecutionContext? executionContext,
            bool modifiesSet)
            : base(null, descriptor, modifiesSet, executionContext)
        {
            Callback = callback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleNode"/> class.
        /// </summary>
        /// <param name="previousNode">The previous node in the chain.</param>
        /// <param name="descriptor">A descriptor for the node.</param>
        /// <param name="callback">The callback to invoke when the node is executed.</param>
        /// <param name="executionContext">An optional execution context (improves description).</param>
        /// <param name="modifiesSet">Indicates whether this node can modify the set of elements passed to it.</param>
        public SingleNode(
            DeclarationNode? previousNode,
            string descriptor,
            Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback,
            TestExecutionContext? executionContext,
            bool modifiesSet)
            : base(previousNode, descriptor, modifiesSet, executionContext)
        {
            Callback = callback;
        }

        /// <summary>
        /// Gets the callback to invoke that executes this node.
        /// </summary>
        public Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> Callback { get; }

        public override ExecutionNode CreateExecutionNode()
        {
            return new SingleExecutionNode(this);
        }
    }
}
