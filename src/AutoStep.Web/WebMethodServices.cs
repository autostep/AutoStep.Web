using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public class WebMethodServices<TConsumer> : IWebMethodServices<TConsumer>
    {
        public WebMethodServices(
            IBrowser browser,
            IConfiguration configuration,
            ILogger<TConsumer> logger,
            IElementChainExecutor chainExecutor,
            MethodContext context,
            IScriptRunner scriptRunner)
        {
            Browser = browser;
            Configuration = configuration;
            Logger = logger;
            ChainExecutor = chainExecutor;
            Context = context;
            ScriptRunner = scriptRunner;
        }

        public IBrowser Browser { get; }

        public IConfiguration Configuration { get; }

        public ILogger Logger { get; }

        public IElementChainExecutor ChainExecutor { get; }

        public MethodContext Context { get; }

        public IScriptRunner ScriptRunner { get; }
    }
}
