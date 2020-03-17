using System;
using System.Linq;
using AutoStep.Assertion;
using AutoStep.Definitions;
using AutoStep.Execution.Interaction;

namespace AutoStep.Web
{
    public class InteractionMethods
    {
        private readonly Browser browser;

        public InteractionMethods(Browser browser)
        {
            this.browser = browser;
        }

        [InteractionMethod("select")]
        public void Select(MethodContext ctxt, string selector)
        {
            var elements = new ElementsQuery(browser);
            ctxt.Elements(elements);

            elements.AddSelect(selector);
        }

        [InteractionMethod("withAttribute")]
        public void WithAttribute(MethodContext ctxt, string attributeName, string attributeValue)
        {
            var elements = ctxt.Elements();

            if(elements is null)
            {
                throw new InvalidOperationException();
            }

            elements.AddWithAttribute(attributeName, attributeValue);
        }

        [InteractionMethod("visible")]
        public void Visible(MethodContext ctxt)
        {
            var elements = ctxt.Elements();

            if (elements is null)
            {
                throw new InvalidOperationException();
            }

            elements.AddVisible();
        }

        [InteractionMethod("withText")]
        public void WithText(MethodContext ctxt, string text)
        {
            var elements = ctxt.Elements();

            if (elements is null)
            {
                throw new InvalidOperationException();
            }

            elements.AddWithText(text);
        }

        [InteractionMethod("getInnerText")]
        public void GetInnerText(MethodContext ctxt)
        {
            var elements = ctxt.Elements();

            if (elements is null)
            {
                throw new InvalidOperationException();
            }

            var first = elements.FirstOrDefault();

            if (first is null)
            {
                throw new AssertionException("No elements selected to get content from.");
            }

            ctxt.ChainValue = first.GetProperty("innerText");
        }

        [InteractionMethod("click")]
        public void Click(MethodContext ctxt)
        {
            var elements = ctxt.Elements();

            if (elements is null)
            {
                throw new InvalidOperationException();
            }

            var first = elements.FirstOrDefault();

            if (first is null)
            {
                throw new AssertionException("No elements selected to click.");
            }

            first.Click();
        }

        [InteractionMethod("type")]
        public void Type(MethodContext ctxt, string text)
        {
            var elements = ctxt.Elements();

            if (elements is null)
            {
                throw new InvalidOperationException();
            }

            var first = elements.FirstOrDefault();

            if (first is null)
            {
                throw new AssertionException("No element available to type into.");
            }

            first.SendKeys(text);
        }

        [InteractionMethod("assertExists")]
        public void AssertExists(MethodContext ctxt)
        {
            var elements = ctxt.Elements();

            if (elements is null)
            {
                throw new InvalidOperationException();
            }

            if(!elements.Any())
            {
                throw new AssertionException("Elements not found.");
            }
        }

        [InteractionMethod("assertText")]
        public void AssertText(MethodContext ctxt, string text)
        {
            var chainContent = ctxt.ChainValue as string;

            if(chainContent is null)
            {
                throw new AssertionException("Expression does not result in a string.");
            }
            else if (!string.Equals(text, chainContent, StringComparison.CurrentCulture))
            {
                throw new AssertionException(string.Format("Found {0}, but expected {1}", chainContent, text));
            }
        }
    }
}
