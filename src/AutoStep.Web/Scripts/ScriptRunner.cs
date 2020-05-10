using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace AutoStep.Web.Scripts
{
    internal class ScriptRunner : IScriptRunner
    {
        private readonly IBrowser browser;
        private readonly IList<IScriptProvider> providers;
        private const string NoModMarker = "__asNOMOD";
        private const string NoFuncMarker = "__asNOFUNC";

        public ScriptRunner(IBrowser browser, IList<IScriptProvider> providers)
        {
            this.browser = browser;
            this.providers = providers;
        }

        public object InvokeFunction(string moduleName, string? functionName, params object?[] args)
        {
            return InvokeInternal(moduleName, functionName, args)!;
        }

        public void InvokeMethod(string moduleName, string? functionName, params object?[] args)
        {
            InvokeInternal(moduleName, functionName, args);
        }

        private object? InvokeInternal(string moduleName, string? functionName, object?[] args)
        {
            // Let's try executing it directly first. If the result of the method we invoke is our special marker, then.
            var jsExecutor = (IJavaScriptExecutor)browser.Driver;

            var result = TryExecutionAttempt(jsExecutor, moduleName, functionName, args);

            var modLoadRequired = false;

            if (result is string strResult)
            {
                if (strResult == NoModMarker)
                {
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

                var complete = CreateCompleteScript(moduleName, module);

                try
                {
                    jsExecutor.ExecuteScript(complete);
                }
                catch (Exception ex)
                {
                    // Error during script creation.
                    throw new ScriptException($"Error occurred loading the {moduleName} module; '{ex.Message}'", ex);
                }

                // Try again.
                result = TryExecutionAttempt(jsExecutor, moduleName, functionName, args);

                if (result is string secondStrResult)
                {
                    if (secondStrResult == NoModMarker)
                    {
                        // Still no module (even after we create it). Something is very wrong.
                        throw new ScriptException($"Unable to load the {moduleName} module.");
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
                throw new ScriptException($"No function {functionName} was found within the {moduleName} module.");
            }
            else
            {
                throw new ScriptException($"The {moduleName} module does not export a default function. A function name should be supplied.");
            }
        }

        private object? TryExecutionAttempt(IJavaScriptExecutor executor, string moduleName, string? functionName, object?[] args)
        {
            // Try and execute, we will then load the mod and try again if we could not.
            string executionAttempt;

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
                // Function within module.
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
                    throw new ScriptException($"Error occurred while invoking the {moduleName}.{functionName} method; '{ex.Message}'.", ex);
                }
                else
                {
                    throw new ScriptException($"Error occurred while invoking the {moduleName} method; '{ex.Message}'.", ex);
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
                throw new ScriptException("No script providers are available.");
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
            throw new ScriptException(string.Format("Could not load requested script module {0}. Looked in: {1}", moduleName, locations));
        }
    }
}
