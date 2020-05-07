using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Execution
{
    /// <summary>
    /// Service for executing element chains.
    /// </summary>
    public interface IElementChainExecutor
    {
        /// <summary>
        /// Execute an element chain, given the start node.
        /// </summary>
        /// <param name="chain">The chain to execute.</param>
        /// <param name="cancellationToken">A cancellation token for stopping execution.</param>
        /// <returns>An awaitable set of elements representing the end of the chain.</returns>
        /// <remarks>
        /// Will throw an exception if the element chain cannot be successfully executed within the retry window defined by the chain options.
        /// </remarks>
        ValueTask<IReadOnlyList<IWebElement>> ExecuteAsync(IElementChain chain, CancellationToken cancellationToken);
    }
}
