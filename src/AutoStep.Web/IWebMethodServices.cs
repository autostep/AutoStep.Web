using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    /// <summary>
    /// Defines the required service by web interaction method classes.
    /// </summary>
    public interface IWebMethodServices
    {
        /// <summary>
        /// Gets the current browser instance.
        /// </summary>
        IBrowser Browser { get; }

        /// <summary>
        /// Gets the chain executor that executes element chains.
        /// </summary>
        IElementChainExecutor ChainExecutor { get; }

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the current interaction method context.
        /// </summary>
        MethodContext Context { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the script runner.
        /// </summary>
        IScriptRunner ScriptRunner { get; }
    }

    /// <summary>
    /// Defines a generic interface that allows us to capture a concrete logger for the consumer type.
    /// </summary>
    /// <typeparam name="TConsumer">The consuming type.</typeparam>
    public interface IWebMethodServices<TConsumer> : IWebMethodServices
    {
    }
}
