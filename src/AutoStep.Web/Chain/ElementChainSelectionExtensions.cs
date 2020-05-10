using System.Collections.Generic;
using System.Linq;
using AutoStep.Assertion;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Extension methods for executing chain operations.
    /// </summary>
    public static class ElementChainSelectionExtensions
    {
        /// <summary>
        /// Add a CSS select operation to the element chain. If there are no previous nodes, this operation searches from the document root. If there are previous nodes,
        /// then it will search inside the elements already in the chain.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="query">The CSS query.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain Select(this IElementChain chain, string query)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrEmpty(query))
            {
                throw new System.ArgumentException(ElementChainMessages.BlankStringParameter, nameof(query));
            }

            if (chain.AnyPreviousNodes)
            {
                return chain.AddNode($"{nameof(Select)}('{query}')", elements =>
                {
                    var set = new List<IWebElement>();

                    foreach (var rootElement in elements)
                    {
                        set.AddRange(rootElement.FindElements(By.CssSelector(query)));
                    }

                    return set;
                });
            }
            else
            {
                return SelectFromRoot(chain, query);
            }
        }

        /// <summary>
        /// Add a CSS select operation to the element chain. This operation searches from the document root.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="query">The CSS query.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain SelectFromRoot(this IElementChain chain, string query)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrEmpty(query))
            {
                throw new System.ArgumentException(ElementChainMessages.BlankStringParameter, nameof(query));
            }

            // No other stages, just search from the root.
            return chain.AddNode($"{nameof(Select)}('{query}')", (elements, browser) =>
            {
                return browser.Driver.FindElements(By.CssSelector(query));
            });
        }

        /// <summary>
        /// Add an attribute filtering operation to the element chain. Elements that have <paramref name="attributeValue"/> for the <paramref name="attributeName"/> attribute
        /// will be included in the output. All other elements will be excluded.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain WithAttribute(this IElementChain chain, string attributeName, string attributeValue)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrEmpty(attributeName))
            {
                throw new System.ArgumentException(ElementChainMessages.BlankStringParameter, nameof(attributeName));
            }

            attributeValue ??= string.Empty;

            return chain.AddNode(
                $"{nameof(WithAttribute)}({attributeName}, {attributeValue})",
                elements => elements.Where(x => x.GetAttribute(attributeName) == attributeValue));
        }

        /// <summary>
        /// Add a filtering operation to the element chain that filters based on the text of the element.
        /// Elements that have an innerText property (sans whitespace) equal to <paramref name="text"/> will be included. All other elements will be excluded.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="text">The attribute value.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain WithText(this IElementChain chain, string text)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            text ??= string.Empty;

            return chain.AddNode($"{nameof(WithText)}('{text}')", elements =>
            {
                return elements.Where(x => x.Text == text);
            });
        }

        /// <summary>
        /// Add a filtering operation to the element chain that filters out hidden elements.
        /// Elements that are visible in the current view-port are considered displayed.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain Displayed(this IElementChain chain)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            return chain.AddNode($"{nameof(Displayed)}()", elements => elements.Where(x => x.Displayed));
        }

        /// <summary>
        /// Filters the set currently in the chain to only the first item.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain First(this IElementChain chain)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            return chain.AddNode($"{nameof(First)}()", elements =>
            {
                return elements.Take(1);
            });
        }
    }
}
