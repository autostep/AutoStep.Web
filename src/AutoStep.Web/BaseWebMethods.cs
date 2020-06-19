using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Configuration;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Declaration;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    /// <summary>
    /// Base class for web interaction methods.
    /// </summary>
    public abstract class BaseWebMethods
    {
        private readonly ElementChainOptions chainOptions;
        private readonly IElementChainExecutor chainExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWebMethods"/> class.
        /// </summary>
        /// <param name="dependencies">The set of dependencies required.</param>
        protected BaseWebMethods(IWebMethodServices dependencies)
        {
            if (dependencies is null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            Browser = dependencies.Browser;
            Logger = dependencies.Logger;
            this.chainExecutor = dependencies.ChainExecutor;
            MethodContext = dependencies.Context;
            ScriptRunner = dependencies.ScriptRunner;
            chainOptions = new ElementChainOptions
            {
                RetryDelayMs = dependencies.Configuration.GetRunValue("web:retryDelayMs", 200),
                PageWaitTimeoutMs = 100,
                TotalRetryTimeoutMs = dependencies.Configuration.GetRunValue("web:totalRetryTimeoutMs", 2000),
            };
        }

        /// <summary>
        /// Gets the active browser instance.
        /// </summary>
        protected IBrowser Browser { get; }

        /// <summary>
        /// Gets a logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the active web driver instance.
        /// </summary>
        protected IWebDriver WebDriver => Browser.Driver;

        /// <summary>
        /// Gets the current method context.
        /// </summary>
        protected MethodContext MethodContext { get; }

        /// <summary>
        /// Gets a script runner for executing named scripts in the browser.
        /// </summary>
        protected IScriptRunner ScriptRunner { get; }

        /// <summary>
        /// Add to the current method chain.
        /// </summary>
        /// <param name="buildCallback">A build callback that can extend the chain.</param>
        /// <returns>The updated chain.</returns>
        protected IElementChain AddToChain(Func<IElementChain, IElementChain>? buildCallback = null)
        {
            IElementChain chain;

            if (MethodContext.ChainValue is DeclarationNode lastNode)
            {
                chain = new ElementChain(lastNode, MethodContext, chainOptions);
            }
            else
            {
                chain = new ElementChain(null, MethodContext, chainOptions);
            }

            if (buildCallback is object)
            {
                chain = buildCallback(chain);

                MethodContext.ChainValue = chain.LeafNode;
            }

            return chain;
        }

        /// <summary>
        /// Execute an element chain.
        /// </summary>
        /// <param name="chain">The chain to execute.</param>
        /// <param name="cancelToken">A cancellation token.</param>
        /// <returns>Awaitable result, outputting a set of elements.</returns>
        protected ValueTask<IReadOnlyList<IWebElement>> ExecuteChainAsync(IElementChain chain, CancellationToken cancelToken)
        {
            return chainExecutor.ExecuteAsync(chain, cancelToken);
        }
    }
}
