using AutoStep.Definitions.Test;
using AutoStep.Execution;
using AutoStep.Execution.Dependency;
using AutoStep.Extensions;
using AutoStep.Projects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public class Extension : IExtensionEntryPoint
    {
        private readonly ILoggerFactory logFactory;

        public Extension(ILoggerFactory logFactory)
        {
            this.logFactory = logFactory;
        }

        public void AttachToProject(IConfiguration projectConfig, Project project)
        {
            project.Compiler.AddStepDefinitionSource(new AssemblyStepDefinitionSource(typeof(Extension).Assembly, logFactory));

            project.Compiler.Interactions.AddMethods<InteractionMethods>();
        }

        public void ExtendExecution(IConfiguration projectConfig, TestRun testRun)
        {
            testRun.Events.Add(new WebEventHandler());
        }

        public void ConfigureExecutionServices(IConfiguration runConfiguration, IServicesBuilder servicesBuilder)
        {
            servicesBuilder.RegisterPerThreadService<IBrowser, Browser>();
        }

        public void Dispose()
        {
        }
    }
}
