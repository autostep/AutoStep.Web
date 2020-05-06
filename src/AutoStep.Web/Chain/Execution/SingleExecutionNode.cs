using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain.Declaration;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Execution
{
    public class SingleExecutionNode : ExecutionNode<SingleNode>
    {
        public SingleExecutionNode(SingleNode node)
            : base(node)
        {
        }

        public override async ValueTask EnterNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken)
        {
            try
            {
                InputElements = inputElements;

                // Throw cancellation here. This ensures that the cancellation exception is captured against the execution node
                // and it looks like an exception inside the callback.
                cancellationToken.ThrowIfCancellationRequested();

                OutputElements = await Node.Callback(inputElements, browser, cancellationToken);
                Error = null;
            }
            catch (Exception ex)
            {
                Error = ex;
                throw;
            }
        }

        public override ValueTask<IReadOnlyList<IWebElement>> ExitNode(IBrowser browser, CancellationToken cancelToken)
        {
            return new ValueTask<IReadOnlyList<IWebElement>>(OutputElements!);
        }
    }
}
