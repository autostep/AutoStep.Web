using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Declaration
{

    public class GroupingNode : DeclarationNode
    {
        public GroupingNode(
            DeclarationNode? previousNode,
            string descriptor,
            TestExecutionContext? executionContext,
            Func<IEnumerable<IReadOnlyList<IWebElement>>, IReadOnlyList<IWebElement>> reducer,
            IEnumerable<IElementChain> chains)
            : base(previousNode, descriptor, true, executionContext)
        {
            Reducer = reducer;
            NestedChains = chains;
        }

        public IEnumerable<IElementChain> NestedChains { get; }

        public Func<IEnumerable<IReadOnlyList<IWebElement>>, IReadOnlyList<IWebElement>> Reducer { get; }

        public override ExecutionNode CreateExecutionNode()
        {
            var executionNodeChildren = new List<ExecutionNode>();

            // Create children.
            // Go through each of the element chains and create our entry point execution node.
            foreach (var item in NestedChains)
            {
                executionNodeChildren.Add(item.CreateExecutionEntryNode());
            }

            // Create the execution nodes.
            return new GroupedExecutionNode(this, executionNodeChildren);
        }
    }
}
