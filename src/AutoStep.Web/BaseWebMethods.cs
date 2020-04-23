using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public class BaseWebMethods
    {
        private readonly ElementChainOptions chainOptions;
        private readonly IElementChainExecutor evaluator;

        public BaseWebMethods(IBrowser browser, ILogger logger, IElementChainExecutor evaluator, MethodContext methodContext)
        {
            Browser = browser;
            Logger = logger;
            this.evaluator = evaluator;
            MethodContext = methodContext;
            chainOptions = new ElementChainOptions
            {
                RetryDelayMs = 200,
                PageWaitTimeoutMs = 100,
                TotalWaitTimeoutMs = 2000
            };
        }

        public IBrowser Browser { get; }

        public ILogger Logger { get; }

        public IWebDriver WebDriver => Browser.Driver;

        public MethodContext MethodContext { get; }

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

        protected ValueTask<IReadOnlyList<IWebElement>> ExecuteChainAsync(IElementChain chain, CancellationToken cancelToken)
        {
            return evaluator.ExecuteAsync(chain, cancelToken);
        }
    }
}
