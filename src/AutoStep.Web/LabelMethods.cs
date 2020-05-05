using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public class LabelMethods : BaseWebMethods
    {
        public LabelMethods(
            IBrowser browser,
            ILogger<LabelMethods> logger,
            IElementChainExecutor evaluator,
            MethodContext methodContext)
            : base(browser, logger, evaluator, methodContext)
        {
        }

        [InteractionMethod("locateElementByStandardLabels")]
        public void LocateElementByStandardLabels(string name)
        {
            AddToChain(c => c.Select("label").WithText(name).AddNode("get elements from labels", elements =>
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
            }));
        }
    }
}
