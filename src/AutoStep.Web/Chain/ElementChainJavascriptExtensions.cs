using System.Collections.Generic;
using AutoStep.Web.Scripts;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Defines chain extension methods for running JS methods.
    /// </summary>
    public static class ElementChainJavascriptExtensions
    {
        /// <summary>
        /// Invoke a javascript function. The first argument will be the current set of elements.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="runner">A script runner.</param>
        /// <param name="moduleName">A module name.</param>
        /// <param name="functionName">A function name.</param>
        /// <param name="args">Any additional arguments to the function.</param>
        /// <returns>A new element chain.</returns>
        public static IElementChain InvokeJavascript(this IElementChain chain, IScriptRunner runner, string moduleName, string functionName, params object?[] args)
        {
            return chain.AddNode($"js.{moduleName}.{functionName}", (elements) =>
            {
                // Insert elements at start of arguments.
                var actualArgs = new object[args.Length + 1];
                actualArgs[0] = elements;
                args.CopyTo(actualArgs, 1);

                var result = runner.InvokeFunction<IReadOnlyList<IWebElement>>(moduleName, functionName, actualArgs);

                if (result is null)
                {
                    // Null result, suggests no change to the set of elements.
                    return elements;
                }

                return result;
            });
        }

        /// <summary>
        /// Invoke a javascript function. The first argument will be the current set of elements.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="runner">A script runner.</param>
        /// <param name="functionName">A function name.</param>
        /// <param name="args">Any additional arguments to the function.</param>
        /// <returns>A new element chain.</returns>
        public static IElementChain InvokeJavascript(this IElementChain chain, IScriptRunner runner, string functionName, params object?[] args)
        {
            return chain.AddNode($"js.{functionName}", (elements) =>
            {
                // Insert elements at start of arguments.
                var actualArgs = new object[args.Length + 1];
                actualArgs[0] = elements;
                args.CopyTo(actualArgs, 1);

                var result = runner.InvokeFunction<IReadOnlyList<IWebElement>>(functionName, null, actualArgs);

                if (result is null)
                {
                    // Null result, suggests no change to the set of elements.
                    return elements;
                }

                return result;
            });
        }
    }
}
