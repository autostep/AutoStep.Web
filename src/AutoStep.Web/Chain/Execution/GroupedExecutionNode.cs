using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Declaration;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Execution
{
    /// <summary>
    /// A grouped execution node, that executes a reducer against the child nodes when the node exits.
    /// </summary>
    public class GroupedExecutionNode : ExecutionNode<GroupingNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedExecutionNode"/> class.
        /// </summary>
        /// <param name="node">The underlying grouping node.</param>
        /// <param name="children">The child execution nodes.</param>
        public GroupedExecutionNode(GroupingNode node, IEnumerable<ExecutionNode> children)
            : base(node)
        {
            ChildNodes = children;
        }

        /// <inheritdoc/>
        public override ValueTask<IReadOnlyList<IWebElement>> ExitNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancelToken)
        {
            try
            {
                // Locate the last node of each child and reduce it using the output of those nodes.
                var result = Node.Reducer(ChildNodes.Select(x => x.Last.OutputElements)!);
                Error = null;

                // Invoke the reducer on the output elements of each of the children.
                return new ValueTask<IReadOnlyList<IWebElement>>(result);
            }
            catch (Exception ex)
            {
                Error = ex;
                throw;
            }
        }
    }
}
