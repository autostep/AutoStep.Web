using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain.Declaration;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Execution
{
    /// <summary>
    /// Represents an execution node for a single callback invocation.
    /// </summary>
    public class SingleExecutionNode : ExecutionNode<SingleNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleExecutionNode"/> class.
        /// </summary>
        /// <param name="node">The declared node to invoke.</param>
        public SingleExecutionNode(SingleNode node)
            : base(node)
        {
        }

        /// <inheritdoc/>
        public override async ValueTask<IReadOnlyList<IWebElement>> ExitNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken)
        {
            try
            {
                // Throw cancellation here. This ensures that the cancellation exception is captured against the execution node
                // and it looks like an exception inside the callback.
                cancellationToken.ThrowIfCancellationRequested();

                var resultElements = await Node.Callback(inputElements, browser, cancellationToken);
                Error = null;
                return resultElements;
            }
            catch (Exception ex)
            {
                Error = ex;
                throw;
            }
        }
    }
}
