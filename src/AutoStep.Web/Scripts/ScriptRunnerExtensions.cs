using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace AutoStep.Web.Scripts
{
    public static class ScriptRunnerExtensions
    {
        public static TResult? InvokeFunction<TResult>(this IScriptRunner runner, string moduleName, string? functionName, params object?[] args)
            where TResult : class
        {
            var expectedType = typeof(TResult);

            object? result = runner.InvokeFunction(moduleName, functionName, args);

            if (result is IEnumerable<object> set)
            {
                // Try converting to sets of web elements if that is what's expected.
                if (typeof(IWebElement[]).IsAssignableFrom(expectedType))
                {
                    result = set.Cast<IWebElement>().ToArray();
                }
                else if (typeof(IList<IWebElement>).IsAssignableFrom(typeof(TResult)))
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
                        throw new ScriptException($"Could not convert the result of {moduleName}.{functionName} to the expected {typeof(TResult).Name}.");
                    }
                    else
                    {
                        throw new ScriptException($"Could not convert the result of {moduleName} to the expected {typeof(TResult).Name}.");
                    }
                }
            }

            return (TResult?)result;
        }
    }
}
