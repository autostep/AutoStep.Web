using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Configuration;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Declaration;
using AutoStep.Web.Chain.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    /// <summary>
    /// Base class for web interaciton methods.
    /// </summary>
    public abstract class BaseWebMethods
    {
        private readonly ElementChainOptions chainOptions;
        private readonly IElementChainExecutor chainExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWebMethods"/> class.
        /// </summary>
        /// <param name="browser">The browser instance.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="chainExecutor">A chain executor.</param>
        /// <param name="methodContext">The current method context.</param>
        protected BaseWebMethods(IBrowser browser, IConfiguration config, ILogger logger, IElementChainExecutor chainExecutor, MethodContext methodContext)
        {
            Browser = browser;
            Logger = logger;
            this.chainExecutor = chainExecutor;
            MethodContext = methodContext;
            chainOptions = new ElementChainOptions
            {
                RetryDelayMs = config.GetRunValue("web:retryDelayMs", 200),
                PageWaitTimeoutMs = 100,
                TotalRetryTimeoutMs = config.GetRunValue("web:totalRetryTimeoutMs", 2000),
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
