using System.Linq;
using AutoStep.Assertion;
using OpenQA.Selenium.Interactions;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Extension methods for executing chain operations.
    /// </summary>
    public static class ElementChainActionExtensions
    {
        /// <summary>
        /// Add a node to the element chain that clicks the first element in the set.
        /// Throws assertion errors if there are no elements, or the selected element is not displayed.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <returns>An updated chain.</returns>
        public static IElementChain Click(this IElementChain chain)
        {
            var query = chain.AddNode(
                $"{nameof(Click)}()",
                elements =>
                {
                    if (elements.Count == 0)
                    {
                        throw new AssertionException($"Expecting an element to click, but no elements found.");
                    }

                    var firstElement = elements[0];

                    if (!firstElement.Displayed)
                    {
                        throw new AssertionException($"Element is not displayed. Cannot click on an invisible element.");
                    }

                    firstElement.Click();
                });

            return query;
        }

        /// <summary>
        /// Adds a node to the element chain that types into the first element in the set, or types directly onto the page if there are no elements.
        /// Throws assertion errors if an element is available, but it is either hidden or disabled.
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="text">The text to type.</param>
        /// <returns>An updated chain.</returns>
        public static IElementChain Type(this IElementChain chain, string text)
        {
            var query = chain.AddNode(
                $"{nameof(Type)}('{text}')",
                (elements, browser) =>
                {
                    if (elements.Count == 0)
                    {
                        // Type on the page.
                        var actions = new Actions(browser.Driver);
                        actions.SendKeys(text);
                        actions.Perform();
                    }
                    else
                    {
                        var firstElement = elements[0];

                        if (!firstElement.Displayed)
                        {
                            throw new AssertionException($"Element is not displayed. Cannot type into an invisible element.");
                        }

                        if (!firstElement.Enabled)
                        {
                            throw new AssertionException($"Element is not enabled. Cannot type in a disabled element.");
                        }

                        firstElement.SendKeys(text);
                    }
                });

            return query;
        }
    }
}
