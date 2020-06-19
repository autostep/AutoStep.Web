using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public interface IWebMethodServices
    {
        IBrowser Browser { get; }

        IElementChainExecutor ChainExecutor { get; }

        IConfiguration Configuration { get; }

        MethodContext Context { get; }

        ILogger Logger { get; }

        IScriptRunner ScriptRunner { get; }
    }

    public interface IWebMethodServices<TConsumer> : IWebMethodServices
    {
    }
}
