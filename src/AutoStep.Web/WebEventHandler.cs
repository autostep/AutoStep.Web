using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Events;

namespace AutoStep.Web
{
    /// <summary>
    /// Provides custom event handling for the web extension.
    /// </summary>
    public class WebEventHandler : BaseEventHandler
    {
        /// <inheritdoc/>
        public override async ValueTask OnFeatureAsync(ILifetimeScope scope, FeatureContext ctxt, Func<ILifetimeScope, FeatureContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            if (nextHandler is null)
            {
                throw new ArgumentNullException(nameof(nextHandler));
            }

            // Create a browser.
            var browser = scope.Resolve<IBrowser>();

            browser.Initialise();

            await nextHandler(scope, ctxt, cancelToken);
        }
    }
}
