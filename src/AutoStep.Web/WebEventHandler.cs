using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Events;
using Microsoft.Extensions.DependencyInjection;

namespace AutoStep.Web
{
    public class WebEventHandler : BaseEventHandler
    {
        public override async ValueTask OnExecuteAsync(ILifetimeScope scope, RunContext ctxt, Func<ILifetimeScope, RunContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            // Add scripts to the script provider.
            await nextHandler(scope, ctxt, cancelToken);
        }

        public override async ValueTask OnFeatureAsync(ILifetimeScope scope, FeatureContext ctxt, Func<ILifetimeScope, FeatureContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            // Create a browser.
            var browser = scope.Resolve<IBrowser>();

            browser.Initialise();

            await nextHandler(scope, ctxt, cancelToken);
        }
    }
}
