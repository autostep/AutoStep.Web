using System;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Events;
using Microsoft.Extensions.DependencyInjection;

namespace AutoStep.Web
{
    public class WebEventHandler : BaseEventHandler
    {
        public override async ValueTask OnExecuteAsync(IServiceProvider scope, RunContext ctxt, Func<IServiceProvider, RunContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            // Add scripts to the script provider.

            await nextHandler(scope, ctxt, cancelToken);
        }

        public override async ValueTask OnFeatureAsync(IServiceProvider scope, FeatureContext ctxt, Func<IServiceProvider, FeatureContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            // Create a browser.
            var browser = scope.GetRequiredService<IBrowser>();

            browser.Initialise();

            await nextHandler(scope, ctxt, cancelToken);
        }
    }
}
