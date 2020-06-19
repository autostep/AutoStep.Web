using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Provides extension methods for invoking scripts and converting the result.
    /// </summary>
    public static class ScriptRunnerExtensions
    {
        /// <summary>
        /// Invoke a named function in a given module and returns the result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The expected return type of the function.
        /// HTML Elements are returned as <see cref="IWebElement"/>, and collections of elements are supported.
        /// </typeparam>
        /// <param name="runner">The script runner.</param>
        /// <param name="moduleName">The name of the module the function is located in. Typically this is the name of the file, minus any path or file extension.</param>
        /// <param name="functionName">
        /// The name of the function exported by <paramref name="moduleName"/> that should be invoked.
        /// If the module uses 'export default' to expose a single method, this value can be null.
        /// </param>
        /// <param name="args">The arguments to the JS function.</param>
        /// <returns>The return value of the function call.</returns>
        public static TResult? InvokeFunction<TResult>(this IScriptRunner runner, string moduleName, string? functionName, params object?[] args)
            where TResult : class
        {
            if (runner is null)
            {
                throw new ArgumentNullException(nameof(runner));
            }

            var expectedType = typeof(TResult);

            object? result = runner.InvokeFunction(moduleName, functionName, args);

            if (result is IEnumerable<object> set)
            {
                // Try converting to sets of web elements if that is what's expected.
                if (typeof(IWebElement[]).IsAssignableFrom(expectedType))
                {
                    result = set.Cast<IWebElement>().ToArray();
                }
                else if (typeof(IReadOnlyList<IWebElement>).IsAssignableFrom(typeof(TResult)))
                {
                    result = set.Cast<IWebElement>().ToList();
                }
                else if (typeof(IEnumerable<IWebElement>).IsAssignableFrom(typeof(TResult)))
                {
                    result = set.Cast<IWebElement>();
                }
            }

            if (result is object)
            {
                if (!expectedType.IsAssignableFrom(result.GetType()))
                {
                    if (functionName is string)
                    {
                        throw new ScriptException(ScriptRunnerMessages.CouldNotConvertFunctionResult.FormatWith(moduleName, functionName, typeof(TResult).Name));
                    }
                    else
                    {
                        throw new ScriptException(ScriptRunnerMessages.CouldNotConvertModuleResult.FormatWith(moduleName, typeof(TResult).Name));
                    }
                }
            }

            return (TResult?)result;
        }
    }
}
