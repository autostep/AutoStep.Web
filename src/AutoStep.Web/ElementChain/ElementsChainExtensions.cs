﻿using System.Collections.Generic;
using System.Linq;
using AutoStep.Assertion;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AutoStep.Web.ElementChain
{
    public static class ElementChainExtensions
    {
        public static IElementChain Select(this IElementChain queryable, string query)
        {
            if (queryable.AnyPreviousStages)
            {
                return queryable.AddStage(elements =>
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
                // No other stages, just search from the root.
                return queryable.AddStage(elements =>
                {
                    return queryable.WebDriver.FindElements(By.CssSelector(query));
                });
            }
        }

        public static IElementChain WithAttribute(this IElementChain queryable, string attributeName, string attributeValue)
        {
            return queryable.AddStage(elements => elements.Where(x => x.GetAttribute(attributeName) == attributeValue));
        }

        public static IElementChain WithText(this IElementChain queryable, string text)
        {
            return queryable.AddStage(elements => elements.Where(x => x.Text == text));
        }

        public static IElementChain Displayed(this IElementChain queryable)
        {
            return queryable.AddStage(elements => elements.Where(x => x.Displayed));
        }

        public static IElementChain AssertSingle(this IElementChain queryable)
        {
            var query = queryable.AddStage(elements =>
            {
                if (elements.SingleOrDefault() is null)
                {
                    throw new AssertionException($"Expecting a single element, but found {elements.Count()}");
                }
            });

            return query;
        }

        public static IElementChain AssertAtLeast(this IElementChain queryable, int minimum)
        {
            var query = queryable.AddStage(elements =>
            {
                if (elements.Count < minimum)
                {
                    throw new AssertionException($"Expecting at least {minimum} element(s), but found {elements.Count}.");
                }
            });

            return query;
        }

        public static IElementChain AssertAttribute(this IElementChain queryable, string attributeName, string attributeValue)
        {
            var query = queryable.AddStage(elements =>
            {
                var firstElement = elements.FirstOrDefault();

                if (firstElement is null)
                {
                    throw new AssertionException($"Expecting an element to assert the {attributeName}, but no elements found.");
                }

                var actualAttributeValue = firstElement.GetAttribute(attributeName);

                if (actualAttributeValue != attributeValue)
                {
                    throw new AssertionException($"Expecting {attributeValue} but found {actualAttributeValue}.");
                }
            });

            return query;
        }

        public static IElementChain First(this IElementChain queryable)
        {
            var query = queryable.AddStage(elements =>
            {
                var element = elements.FirstOrDefault();

                if (element is null)
                {
                    throw new AssertionException($"Expecting an element, but found none.");
                }
            });

            return query;
        }

        public static IElementChain Click(this IElementChain queryable)
        {
            var query = queryable.AddStage(elements =>
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
            var query = queryable.AddStage(elements =>
            {
                var firstElement = elements.FirstOrDefault();

                if (firstElement is null)
                {
                    // Type on the page.
                    var actions = new Actions(queryable.WebDriver);
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
