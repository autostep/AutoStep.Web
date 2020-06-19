using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoStep.Assertion;
using AutoStep.Elements.Metadata;
using AutoStep.Execution;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoStep.ExtensionTesting
{
    public class ErrorEventHandler : BaseEventHandler
    {
        public const string ExpectingErrorName = "expectingError";

        public override async ValueTask OnScenarioAsync(ILifetimeScope scope, ScenarioContext ctxt, Func<ILifetimeScope, ScenarioContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            var logger = scope.Resolve<ILogger<ErrorEventHandler>>();

            // Check the options on the scenario.
            var expectingErrorOption = ctxt.Scenario.Annotations.OfType<IOptionInfo>().FirstOrDefault(x => x.Name.Equals(ExpectingErrorName, StringComparison.CurrentCultureIgnoreCase));

            Exception encounteredError = null;
            var thrown = false;

            try
            {
                await nextHandler(scope, ctxt, cancelToken);

                encounteredError = ctxt.FailException;
            }
            catch(Exception ex)
            {
                encounteredError = ex;
                thrown = true;
            }

            // Expand out the error to make sure we consume the right one.
            if (encounteredError is StepFailureException || encounteredError is EventHandlingException)
            {
                if (encounteredError.InnerException is object)
                {
                    encounteredError = encounteredError.InnerException;
                }
            }

            if (expectingErrorOption is object)
            {
                if (encounteredError is null)
                {
                    if (expectingErrorOption.Setting is null)
                    {
                        // No error; throw.
                        ctxt.FailException = new AssertionException("Expected the scenario to fail, but the scenario passed.");
                    }
                    else
                    {
                        // No error; throw.
                        ctxt.FailException = new AssertionException("Expected the scenario to fail, with the message '" + expectingErrorOption.Setting + "', but the scenario passed.");
                    }
                }
                else
                {
                    if (expectingErrorOption.Setting is object && encounteredError.Message != expectingErrorOption.Setting)
                    {
                        ctxt.FailException = new AssertionException("Expected the scenario to fail, with the message '" + expectingErrorOption.Setting + "', but actual message was '" + encounteredError.Message + "'.");
                    }
                    else
                    {
                        // No message expectation, treat the scenario as passing.
                        // Log information to that effect.
                        logger.LogInformation("Error in scenario was expected. Scenario was a success.");
                        ctxt.FailException = null;
                        ctxt.FailingStep = null;
                    }
                }
            }
            else if (thrown)
            {
                throw encounteredError;
            }
        }
    }
}
