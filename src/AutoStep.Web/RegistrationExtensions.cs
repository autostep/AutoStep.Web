using System;
using System.Collections.Generic;
using System.Text;
using AutoStep.Definitions;
using AutoStep.Definitions.Test;
using AutoStep.Execution;
using AutoStep.Language;
using AutoStep.Projects;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public static class RegistrationExtensions
    {
        public static void AddWebInteractions(this Project project, ILoggerFactory loggerFactory)
        {
            project.Compiler.AddStaticStepDefinitionSource(new AssemblyStepDefinitionSource(typeof(RegistrationExtensions).Assembly, loggerFactory));

            project.Compiler.Interactions.AddMethods<InteractionMethods>();

            project.TryAddFile(new ProjectInteractionFile("__defaultInteractions", new StringContentSource(InteractionFiles.DefaultInteractions)));
        }

        public static void AddWebHandlers(this TestRun testRun)
        {
            testRun.Events.Add(new WebEventHandler());
        }
    }
}
