using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public class LabelMethods : BaseWebMethods
    {
        public LabelMethods(
            IBrowser browser,
            IConfiguration configuration,
            ILogger<LabelMethods> logger,
            IElementChainExecutor evaluator,
            MethodContext methodContext)
            : base(browser, configuration, logger, evaluator, methodContext)
        {
        }

        [InteractionMethod("locateInputByStandardLabels")]
        public void LocateInputByStandardLabels(string name)
        {
            AddToChain(c => c.Union(
                chain => chain.Select("label").WithText(name).Union(
                    nested => nested.AddNode("get elements from for attribute", elements =>
                    {
                        // Elements present are those with the right text.
                        // Get the 'for' value for each element and then find all elements with those IDs.
                        var ids = elements.Select(x => x.GetAttribute("for")).Where(id => !string.IsNullOrEmpty(id)).ToList();

                        if (ids.Count == 0)
                        {
                            return Array.Empty<IWebElement>();
                        }

                        // Find all elements with that ID.
                        return Browser.Driver.FindElements(By.CssSelector("#" + string.Join(",#", ids)));
                    }),
                    nested => nested.AddNode("get elements from aria-labelledby", elements =>
                    {
                        var ids = elements.Select(x => x.GetAttribute("id")).Where(id => !string.IsNullOrEmpty(id)).ToList();

                        if (ids.Count == 0)
                        {
                            return Array.Empty<IWebElement>();
                        }

                        return Browser.Driver.FindElements(By.CssSelector(string.Join(",", ids.Select(id => $"input[aria-labelledby='{id}']"))));
                    })),
                chain => chain.Select("input").WithAttribute("aria-label", name)));
        }
    }
}
