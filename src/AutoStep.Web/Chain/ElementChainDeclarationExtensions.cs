using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    public static class ElementChainDeclarationExtensions
    {
        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IEnumerable<IWebElement>>> callback)
        {
            return chain.AddNode(descriptor, async (webElements, browser, cancelToken) =>
            {
                var results = await callback(webElements, browser, cancelToken);

                // Need to evaluate at each stage.
                return results.ToList();
            });
        }

        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IEnumerable<IWebElement>>> callback)
        {
            return chain.AddNode(descriptor, async (webElements, browser, cancelToken) =>
            {
                var results = await callback(webElements, cancelToken);

                // Need to evaluate at each stage.
                return results.ToList();
            });
        }

        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, IEnumerable<IWebElement>> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                return new ValueTask<IReadOnlyList<IWebElement>>(callback(webElements, browser).ToList());
            });
        }

        public static IElementChain AddNode(this IElementChain chain, string descriptor, Func<IReadOnlyList<IWebElement>, IEnumerable<IWebElement>> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                return new ValueTask<IReadOnlyList<IWebElement>>(callback(webElements).ToList());
            });
        }

        public static IElementChain AddNode(this IElementChain chain, string descriptor, Action<IReadOnlyList<IWebElement>, IBrowser> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                callback(webElements, browser);

                return default(ValueTask);
            });
        }

        public static IElementChain AddNode(this IElementChain chain, string descriptor, Action<IReadOnlyList<IWebElement>> callback)
        {
            return chain.AddNode(descriptor, (webElements, browser, cancelToken) =>
            {
                // Need to evaluate at each stage.
                callback(webElements);

                return default(ValueTask);
            });
        }
    }
}
