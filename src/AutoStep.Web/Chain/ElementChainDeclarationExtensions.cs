using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Extension methods for adding callbacks with varying signatures to the element chain.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1062:Validate arguments of public methods",
        Justification = "Analyser doesn't understand that the AssertArguments checks everything.")]
    public static class ElementChainDeclarationExtensions
    {
        /// <summary>
        /// Adds a node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The async callback to invoke (that takes an <see cref="IBrowser"/> instance).</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IEnumerable<IWebElement>>> callback)
        {
            AssertArguments(chain, descriptor, callback);

            return chain.AddNode(descriptor, async (webElements, browser, cancelToken) =>
            {
                var results = await callback(webElements, browser, cancelToken);

                // Need to evaluate at each stage.
                return results.ToList();
            });
        }

        /// <summary>
        /// Adds a node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The async callback to invoke.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IEnumerable<IWebElement>>> callback)
        {
            return chain.AddNode(descriptor, async (webElements, browser, cancelToken) =>
            {
                var results = await callback(webElements, cancelToken);

                // Need to evaluate at each stage.
                return results.ToList();
            });
        }

        /// <summary>
        /// Adds a node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The callback to invoke (that takes an <see cref="IBrowser"/> instance).</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, IEnumerable<IWebElement>> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                return new ValueTask<IReadOnlyList<IWebElement>>(callback(webElements, browser).ToList());
            });
        }

        /// <summary>
        /// Adds a node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, IEnumerable<IWebElement>> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                return new ValueTask<IReadOnlyList<IWebElement>>(callback(webElements).ToList());
            });
        }

        /// <summary>
        /// Adds a non-modifying node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Action<IReadOnlyList<IWebElement>, IBrowser> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                callback(webElements, browser);

                return default(ValueTask);
            });
        }

        /// <summary>
        /// Adds a non-modifying node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Action<IReadOnlyList<IWebElement>> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                callback(webElements);

                return default(ValueTask);
            });
        }

        private static void AssertArguments(IElementChain chain, string descriptor, Delegate callback)
        {
            if (chain is null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrWhiteSpace(descriptor))
            {
                throw new ArgumentException("Descriptor cannot be null or whitespace.", nameof(descriptor));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
        }
    }
}
