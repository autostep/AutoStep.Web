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
    public class GroupedExecutionNode : ExecutionNode<GroupingNode>
    {
        public GroupedExecutionNode(GroupingNode node, IEnumerable<ExecutionNode> children)
            : base(node)
        {
            Children = children;
        }

        public override ValueTask EnterNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken)
        {
            // Set the input elements.
            InputElements = inputElements;

            // Throw cancellation here. This ensures that the cancellation exception is captured against the execution node
            // and it looks like an exception inside the callback.
            cancellationToken.ThrowIfCancellationRequested();

            return default;
        }

        public override ValueTask<IReadOnlyList<IWebElement>> ExitNode(IBrowser browser, CancellationToken cancelToken)
        {
            try
            {
                // Locate the last node of each child and reduce it using the output of those nodes.
                OutputElements = Node.Reducer(Children.Select(x => x.Last.OutputElements)!);
                Error = null;
            }
            catch (Exception ex)
            {
                Error = ex;
                throw;
            }

            // Invoke the reducer on the output elements of each of the children.
            return new ValueTask<IReadOnlyList<IWebElement>>(OutputElements);
        }
    }
}
