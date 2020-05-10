using System;
using System.Linq;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    /// <summary>
    /// Defines methods for working with field labels.
    /// </summary>
    public class LabelMethods : BaseWebMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LabelMethods"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="chainExecutor">A chain executor.</param>
        /// <param name="methodContext">The active method context.</param>
        public LabelMethods(
            IBrowser browser,
            IConfiguration configuration,
            ILogger<LabelMethods> logger,
            IElementChainExecutor chainExecutor,
            MethodContext methodContext)
            : base(browser, configuration, logger, chainExecutor, methodContext)
        {
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Interaction method docs comes from an attribute; don't want to duplicate info.

        [InteractionMethod("locateInputByStandardLabels", Documentation = @"
            
            Attempts to locate input fields using a set of common application behaviours.

            Inputs will be found based on their label's in three different ways:

            1. Labels with a designated for attribute:  

               ```html
               <label for=""field1"">Field 1</label>
               <input type=""text"" id=""field1"" />
               ```  

            2. Labels referenced using the aria-labelledby attribute:

               ```html
               <label id=""field1Label"">Field 1</label>
               <input type=""text"" aria-labelledby=""field1Label"" />

            3. Using the aria-label attribute:
        
               ```html
               <input type=""text"" aria-label=""Field 1"" />
               ```

            All matched inputs will be included in the set.

        ")]
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
