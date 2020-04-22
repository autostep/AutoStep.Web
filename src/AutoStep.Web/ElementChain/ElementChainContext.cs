using AutoStep.Web.Queryable;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web.ElementChain
{
    internal class ElementChainContext
    {
        public ElementChainContext(IBrowser browser, ILogger logger, IElementChainDescriber describer, ElementChainOptions options)
        {
            Browser = browser;
            Logger = logger;
            Describer = describer;
            Options = options;
        }

        public IBrowser Browser { get; }

        public ILogger Logger { get; }

        public ElementChainOptions Options { get; }

        public IElementChainDescriber Describer { get; }
    }
}
