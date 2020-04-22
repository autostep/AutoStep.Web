using System;
using AutoStep.Execution.Contexts;
using AutoStep.Web.ElementChain;
using AutoStep.Web.Queryable;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public class BaseWebMethods
    {
        private readonly Lazy<ElementChainContext> chainContext;

        public BaseWebMethods(IBrowser browser, ILogger logger, MethodContext methodContext)
        {
            Browser = browser;
            Logger = logger;
            MethodContext = methodContext;
            chainContext = new Lazy<ElementChainContext>(() => new ElementChainContext(Browser, Logger, new ElementChainDescriber(), new ElementChainOptions
            {
                RetryDelayMs = 200,
                PageWaitTimeoutMs = 100,
                TotalWaitTimeoutMs = 2000
            }));
        }

        public IBrowser Browser { get; }

        public ILogger Logger { get; }

        public MethodContext MethodContext { get; }

        public IElementChain Chain(Func<IElementChain, IElementChain>? buildCallback = null)
        {
            IElementChain chain;

            if (MethodContext.ChainValue is ElementChainNode lastNode)
            {
                chain = new ElementChainBuilder(lastNode, MethodContext, chainContext.Value);
            }
            else
            {
                chain = new ElementChainBuilder(null, MethodContext, chainContext.Value);
            }

            if (buildCallback is object)
            {
                chain = buildCallback(chain);

                MethodContext.ChainValue = chain.LastNode;
            }

            return chain;
        }
    }
}
