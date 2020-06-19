using System;
using System.Collections.Generic;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Declaration
{
    /// <summary>
    /// A grouping node in an element chain.
    /// </summary>
    /// <remarks>
    /// Grouping nodes contain multiple nested element chains, each of which execute in order with the same inputs.
    /// When all the chains have executed successfully, the set of elements returned from each chain is provided to a reducer
    /// function, that outputs a single set of elements as a result.
    /// </remarks>
    public class GroupingNode : DeclarationNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingNode"/> class.
        /// </summary>
        /// <param name="previousNode">The previous node in the chain. If null, this is the first node in the chain.</param>
        /// <param name="descriptor">A piece of descriptive text for the node, similar to a method name, used when reporting chain diagnostics.</param>
        /// <param name="executionContext">An optional execution context for the caller (used to improve descriptive output).</param>
        /// <param name="reducer">
        /// A reducer function, that takes in an enumerable of element sets, and outputs a single, reduced set.
        /// Typically, reducer function will combine the input sets (concatenate, union, etc).
        /// </param>
        /// <param name="nestedChains">The nested set of element chains. Each chain executes separately.</param>
        public GroupingNode(
            DeclarationNode? previousNode,
            string descriptor,
            TestExecutionContext? executionContext,
            Func<IEnumerable<IReadOnlyList<IWebElement>>, IReadOnlyList<IWebElement>> reducer,
            IEnumerable<IElementChain> nestedChains)
            : base(previousNode, descriptor, modifiesSet: true, executionContext)
        {
            Reducer = reducer;
            NestedChains = nestedChains;
        }

        /// <summary>
        /// Gets the set of nested chains.
        /// </summary>
        public IEnumerable<IElementChain> NestedChains { get; }

        /// <summary>
        /// Gets the reducer function for this node.
        /// </summary>
        public Func<IEnumerable<IReadOnlyList<IWebElement>>, IReadOnlyList<IWebElement>> Reducer { get; }

        /// <inheritdoc/>
        public override ExecutionNode CreateExecutionNode()
        {
            var executionNodeChildren = new List<ExecutionNode>();

            // Create children.
            // Go through each of the element chains and create our entry point execution node.
            foreach (var item in NestedChains)
            {
                var executionNode = item.CreateExecutionEntryNode();

                if (executionNode is object)
                {
                    executionNodeChildren.Add(executionNode);
                }
            }

            // Create the execution node (with the given execution child nodes).
            return new GroupedExecutionNode(this, executionNodeChildren);
        }
    }
}
