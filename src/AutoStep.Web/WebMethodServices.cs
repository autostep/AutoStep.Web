using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    /// <summary>
    /// Provides the generic web method services instance that collects the required dependencies.
    /// </summary>
    /// <typeparam name="TConsumer">The consuming type.</typeparam>
    internal class WebMethodServices<TConsumer> : IWebMethodServices<TConsumer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebMethodServices{TConsumer}"/> class.
        /// </summary>
        /// <param name="browser">The browser instance.</param>
        /// <param name="configuration">The current configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="chainExecutor">The chain executor provider.</param>
        /// <param name="context">The method context.</param>
        /// <param name="scriptRunner">The script runner.</param>
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

        /// <inheritdoc/>
        public IBrowser Browser { get; }

        /// <inheritdoc/>
        public IConfiguration Configuration { get; }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        /// <inheritdoc/>
        public IElementChainExecutor ChainExecutor { get; }

        /// <inheritdoc/>
        public MethodContext Context { get; }

        /// <inheritdoc/>
        public IScriptRunner ScriptRunner { get; }
    }
}
