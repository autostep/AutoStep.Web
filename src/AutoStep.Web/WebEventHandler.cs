using System;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Events;

namespace AutoStep.Web
{
    public class WebEventHandler : BaseEventHandler
    {
        public override async ValueTask OnFeature(IServiceScope scope, FeatureContext ctxt, Func<IServiceScope, FeatureContext, ValueTask> nextHandler)
        {
            // Create a browser.
            var browser = scope.Resolve<Browser>();

            browser.Initialise();

            await nextHandler(scope, ctxt);
        }
    }
}
