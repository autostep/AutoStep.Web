using System;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Events;
using Microsoft.Extensions.DependencyInjection;

namespace AutoStep.Web
{
    public class WebEventHandler : IEventHandler
    {
        public ValueTask OnExecute(IServiceProvider scope, RunContext ctxt, Func<IServiceProvider, RunContext, ValueTask> nextHandler)
        {
            return nextHandler(scope, ctxt);
        }

        public async ValueTask OnFeature(IServiceProvider scope, FeatureContext ctxt, Func<IServiceProvider, FeatureContext, ValueTask> nextHandler)
        {
            // Create a browser.
            var browser = scope.GetRequiredService<Browser>();

            browser.Initialise();

            await nextHandler(scope, ctxt);
        }

        public ValueTask OnScenario(IServiceProvider scope, ScenarioContext ctxt, Func<IServiceProvider, ScenarioContext, ValueTask> nextHandler)
        {
            return nextHandler(scope, ctxt);
        }

        public ValueTask OnStep(IServiceProvider scope, StepContext ctxt, Func<IServiceProvider, StepContext, ValueTask> nextHandler)
        {
            return nextHandler(scope, ctxt);
        }

        public ValueTask OnThread(IServiceProvider scope, ThreadContext ctxt, Func<IServiceProvider, ThreadContext, ValueTask> nextHandler)
        {
            return nextHandler(scope, ctxt);
        }
    }
}
