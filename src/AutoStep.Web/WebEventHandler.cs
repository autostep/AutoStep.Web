using System;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Events;
using Microsoft.Extensions.DependencyInjection;

namespace AutoStep.Web
{
    public class WebEventHandler : BaseEventHandler
    {
        public override async ValueTask OnFeature(IServiceProvider scope, FeatureContext ctxt, Func<IServiceProvider, FeatureContext, ValueTask> nextHandler)
        {
            // Create a browser.
            var browser = scope.GetRequiredService<Browser>();

            browser.Initialise();

            await nextHandler(scope, ctxt);
        }
    }
}
