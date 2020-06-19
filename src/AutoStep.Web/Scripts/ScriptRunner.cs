using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Provides the functionality to locate and load built JS modules, inject them into the browser, and invoke them.
    /// </summary>
    internal class ScriptRunner : IScriptRunner
    {
        private const string NoModMarker = "__asNOMOD";
        private const string NoFuncMarker = "__asNOFUNC";

        private readonly IBrowser browser;
        private readonly ILogger<ScriptRunner> logger;
        private readonly IList<IScriptProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptRunner"/> class.
        /// </summary>
        /// <param name="browser">The browser instance to inject into.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="providers">The set of configured script providers.</param>
        public ScriptRunner(IBrowser browser, ILogger<ScriptRunner> logger, IList<IScriptProvider> providers)
        {
            this.browser = browser;
            this.logger = logger;
            this.providers = providers;
        }

        /// <inheritdoc/>
        public object InvokeFunction(string moduleName, string? functionName, params object?[] args)
        {
            return InvokeInternal(moduleName, functionName, args)!;
        }

        /// <inheritdoc/>
        public void InvokeMethod(string moduleName, string? functionName, params object?[] args)
        {
            InvokeInternal(moduleName, functionName, args);
        }

        private object? InvokeInternal(string moduleName, string? functionName, object?[] args)
        {
            var jsExecutor = (IJavaScriptExecutor)browser.Driver;

            // Let's try executing it directly first. If the result of the method we invoke is one of our special markers, we
            // can do different things.
            var result = TryExecutionAttempt(jsExecutor, moduleName, functionName, args);

            var modLoadRequired = false;

            if (result is string strResult)
            {
                if (strResult == NoModMarker)
                {
                    // Our marker was received indicating that the requested module is not loaded.
                    logger.LogDebug(ScriptRunnerMessages.ModuleNotLoaded, moduleName);
                    modLoadRequired = true;
                }
                else if (strResult == NoFuncMarker)
                {
                    // No function with that name; error.
                    ThrowNoFuncError(moduleName, functionName);
                }
            }

            if (modLoadRequired)
            {
                // No module; load it into the browser.
                var module = LoadModule(moduleName);

                // Get the complete script (with wrappers), that loads the module into the browser.
                var complete = CreateCompleteScript(moduleName, module);

                try
                {
                    jsExecutor.ExecuteScript(complete);
                }
                catch (Exception ex)
                {
                    // Error during script creation.
                    throw new ScriptException(ScriptRunnerMessages.ErrorLoadingModule.FormatWith(moduleName, ex.Message), ex);
                }

                // Try again.
                result = TryExecutionAttempt(jsExecutor, moduleName, functionName, args);

                if (result is string secondStrResult)
                {
                    if (secondStrResult == NoModMarker)
                    {
                        // Still no module (even after we create it). Something is very wrong.
                        throw new ScriptException(ScriptRunnerMessages.UnableToDetectedLoadedModule.FormatWith(moduleName));
                    }
                    else if (secondStrResult == NoFuncMarker)
                    {
                        // No function with that name; error.
                        ThrowNoFuncError(moduleName, functionName);
                    }
                }
            }

            return result;
        }

        private static void ThrowNoFuncError(string moduleName, string? functionName)
        {
            // No function with that name; error.
            if (functionName is string)
            {
                throw new ScriptException(ScriptRunnerMessages.FunctionNotFound.FormatWith(functionName, moduleName));
            }
            else
            {
                throw new ScriptException(ScriptRunnerMessages.DefaultModuleFunctionNotFound.FormatWith(moduleName));
            }
        }

        private object? TryExecutionAttempt(IJavaScriptExecutor executor, string moduleName, string? functionName, object?[] args)
        {
            // Try and execute, we will then load the mod and try again if we could not.
            string executionAttempt;

            if (functionName is string)
            {
                logger.LogDebug(ScriptRunnerMessages.InvokingFunctionScript, moduleName, functionName);
            }
            else
            {
                logger.LogDebug(ScriptRunnerMessages.InvokingModuleScript, moduleName);
            }

            if (functionName is string)
            {
                // Function within module.
                executionAttempt = "var h = window.asmod, a = arguments;" +
                                   $"if(h && h.{moduleName}) {{" +
                                   $"var s = h.{moduleName}.exports.{functionName};" +
                                   $"return s instanceof Function ? s({GetArgumentList(args.Length)}) : '{NoFuncMarker}'; }}" +
                                   $"return '{NoModMarker}';";
            }
            else
            {
                // Module is a function.
                executionAttempt = "var h = window.asmod, a = arguments;" +
                                   $"if(h && h.{moduleName}) {{" +
                                   $"var s = h.{moduleName}.exports;" +
                                   $"return s instanceof Function ? s({GetArgumentList(args.Length)}) : '{NoFuncMarker}'; }}" +
                                   $"return '{NoModMarker}';";
            }

            try
            {
                return executor.ExecuteScript(executionAttempt, args);
            }
            catch (Exception ex)
            {
                if (functionName is string)
                {
                    throw new ScriptException(ScriptRunnerMessages.ErrorInvokingFunctionScript.FormatWith(moduleName, functionName, ex.Message), ex);
                }
                else
                {
                    throw new ScriptException(ScriptRunnerMessages.ErrorInvokingModuleScript.FormatWith(moduleName, ex.Message), ex);
                }
            }
        }

        private static string GetArgumentList(int argsCount)
        {
            return string.Join(',', Enumerable.Range(0, argsCount).Select(i => $"a[{i}]"));
        }

        private string CreateCompleteScript(string moduleName, string moduleBody)
        {
            return $"var h = window.asmod = (window.asmod || {{}}), m = h.{moduleName}=(h.{moduleName}|| {{exports:{{}}}});" +
                   "(function(module, exports) {" +
                   moduleBody +
                   "})(m, m.exports);";
        }

        private string LoadModule(string moduleName)
        {
            if (providers.Count == 0)
            {
                throw new ScriptException(ScriptRunnerMessages.NoScriptProvidersAvailable);
            }

            foreach (var provider in providers)
            {
                if (provider.TryGetScriptModule(moduleName, out var content))
                {
                    return content;
                }
            }

            var linePrefix = Environment.NewLine + " - ";
            var locations = linePrefix + string.Join(Environment.NewLine + " - ", providers.Select(x => x.GetLocationDescription(moduleName)));

            // Could not load the module.
            throw new ScriptException(ScriptRunnerMessages.CouldNotLoadModule.FormatWith(moduleName, locations));
        }
    }
}
