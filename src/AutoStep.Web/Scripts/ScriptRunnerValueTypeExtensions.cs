using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Extension methods for invoking a JS function that returns value types.
    /// </summary>
    public static class ScriptRunnerValueTypeExtensions
    {
        /// <summary>
        /// Invoke a named function in a given module and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The expected return type of the function.</typeparam>
        /// <param name="runner">The script runner.</param>
        /// <param name="moduleName">The name of the module the function is located in. Typically this is the name of the file, minus any path or file extension.</param>
        /// <param name="functionName">
        /// The name of the function exported by <paramref name="moduleName"/> that should be invoked.
        /// If the module uses 'export default' to expose a single method, this value can be null.
        /// </param>
        /// <param name="args">The arguments to the JS function.</param>
        /// <returns>The return value of the function call.</returns>
        public static TResult InvokeFunction<TResult>(this IScriptRunner runner, string moduleName, string? functionName, params object?[] args)
           where TResult : struct
        {
            if (runner is null)
            {
                throw new ArgumentNullException(nameof(runner));
            }

            var expectedType = typeof(TResult);

            object? result = runner.InvokeFunction(moduleName, functionName, args);

            if (result is null || !expectedType.IsAssignableFrom(result.GetType()))
            {
                if (functionName is string)
                {
                    throw new ScriptException($"Could not convert the result of {moduleName}.{functionName} to the expected {typeof(TResult).Name}.");
                }
                else
                {
                    throw new ScriptException($"Could not convert the result of {moduleName} to the expected {typeof(TResult).Name}.");
                }
            }

            return (TResult)result;
        }
    }
}
