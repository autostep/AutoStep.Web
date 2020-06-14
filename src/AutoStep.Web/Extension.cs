using AutoStep.Configuration;
using AutoStep.Definitions.Test;
using AutoStep.Execution;
using AutoStep.Execution.Dependency;
using AutoStep.Extensions;
using AutoStep.Projects;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
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
        private readonly IAutoStepEnvironment environment;
        private readonly IPackageMetadata extensionMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="Extension"/> class.
        /// </summary>
        /// <param name="logFactory">A log factory.</param>
        /// <param name="environment">The AutoStep environment.</param>
        /// <param name="extensionMetadata">The extension metadata.</param>
        public Extension(ILoggerFactory logFactory, IAutoStepEnvironment environment, IPackageMetadata extensionMetadata)
        {
            this.logFactory = logFactory;
            this.environment = environment;
            this.extensionMetadata = extensionMetadata;
        }

        /// <inheritdoc/>
        public void AttachToProject(IConfiguration projectConfig, Project project)
        {
            if (project is null)
            {
                throw new System.ArgumentNullException(nameof(project));
            }

            project.Builder.AddStepDefinitionSource(new AssemblyStepDefinitionSource(typeof(Extension).Assembly, logFactory));

            project.Builder.Interactions.AddMethods<InteractionMethods>();
            project.Builder.Interactions.AddMethods<LabelMethods>();
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

            var useMinifiedScripts = runConfiguration.GetRunValue("web:useMinifiedScripts", true);

            // If not local, add the scripts in our package folder.
            if (!(extensionMetadata is ILocalExtensionPackageMetadata))
            {
                var packageScriptsFolder = extensionMetadata.GetPath("content", "scripts");
                servicesBuilder.RegisterInstance<IScriptProvider>(new FolderScriptProvider(environment, packageScriptsFolder, useMinifiedScripts));
            }

            // Add any configured scripts.
            var scriptsFolder = runConfiguration.GetRunValue<string?>("web:scripts", null);

            if (scriptsFolder is string)
            {
                servicesBuilder.RegisterInstance<IScriptProvider>(new FolderScriptProvider(environment, scriptsFolder, useMinifiedScripts));
            }

            servicesBuilder.RegisterPerThreadService<IScriptRunner, ScriptRunner>();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
