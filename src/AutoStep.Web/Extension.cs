using AutoStep.Definitions.Test;
using AutoStep.Execution;
using AutoStep.Execution.Dependency;
using AutoStep.Extensions;
using AutoStep.Projects;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    /// <summary>
    /// Extension entry point.
    /// </summary>
    public sealed class Extension : IExtensionEntryPoint
    {
        private readonly ILoggerFactory logFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Extension"/> class.
        /// </summary>
        /// <param name="logFactory">A log factory.</param>
        public Extension(ILoggerFactory logFactory)
        {
            this.logFactory = logFactory;
        }

        /// <inheritdoc/>
        public void AttachToProject(IConfiguration projectConfig, Project project)
        {
            if (project is null)
            {
                throw new System.ArgumentNullException(nameof(project));
            }

            project.Compiler.AddStepDefinitionSource(new AssemblyStepDefinitionSource(typeof(Extension).Assembly, logFactory));

            project.Compiler.Interactions.AddMethods<InteractionMethods>();
            project.Compiler.Interactions.AddMethods<LabelMethods>();
        }

        /// <inheritdoc/>
        public void ExtendExecution(IConfiguration projectConfig, TestRun testRun)
        {
            if (testRun is null)
            {
                throw new System.ArgumentNullException(nameof(testRun));
            }

            testRun.Events.Add(new WebEventHandler());
        }

        /// <inheritdoc/>
        public void ConfigureExecutionServices(IConfiguration runConfiguration, IServicesBuilder servicesBuilder)
        {
            if (servicesBuilder is null)
            {
                throw new System.ArgumentNullException(nameof(servicesBuilder));
            }

            servicesBuilder.RegisterPerThreadService<IBrowser, Browser>();
            servicesBuilder.RegisterPerResolveService<IElementChainExecutor, ChainExecutor>();
            servicesBuilder.RegisterSingleton<IChainDescriber, ChainDescriber>();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
