using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace AutoStep.Web.Scripts
{
    public static class ScriptRunnerValueTypeExtensions
    {
        public static TResult InvokeFunction<TResult>(this IScriptRunner runner, string moduleName, string? functionName, params object?[] args)
           where TResult : struct
        {
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
