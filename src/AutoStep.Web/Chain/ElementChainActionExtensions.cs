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
        public static IElementChain Click(this IElementChain queryable)
        {
            var query = queryable.AddNode(
                $"{nameof(Click)}()",
                elements =>
                {
                    var firstElement = elements.FirstOrDefault();

                    if (firstElement is null)
                    {
                        throw new AssertionException($"Expecting an element to click, but no elements found.");
                    }

                    if (!firstElement.Displayed)
                    {
                        throw new AssertionException($"Element is not displayed. Cannot click on an invisible element.");
                    }

                    firstElement.Click();
                });

            return query;
        }

        public static IElementChain Type(this IElementChain queryable, string text)
        {
            var query = queryable.AddNode(
                $"{nameof(Type)}('{text}')",
                (elements, browser) =>
                {
                    var firstElement = elements.FirstOrDefault();

                    if (firstElement is null)
                    {
                        // Type on the page.
                        var actions = new Actions(browser.Driver);
                        actions.SendKeys(text);
                        actions.Perform();
                    }
                    else
                    {
                        if (!firstElement.Enabled)
                        {
                            throw new AssertionException($"Element is not enabled. Cannot type in an invisible element.");
                        }

                        firstElement.SendKeys(text);
                    }
                });

            return query;
        }
    }
}
