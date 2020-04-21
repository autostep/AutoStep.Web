using AutoStep.Web.Queryable;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web.ElementChain
{
    internal class ElementChainContext
    {
        public ElementChainContext(IBrowser browser, ILogger logger, ElementChainOptions options)
        {
            Browser = browser;
            Logger = logger;
            Options = options;
        }

        public IBrowser Browser { get; }

        public ILogger Logger { get; }

        public ElementChainOptions Options { get; }
    }
}
