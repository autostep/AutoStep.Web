using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public class InteractionMethods : BaseWebMethods
    {
        public InteractionMethods(IBrowser browser, IConfiguration config, ILogger<InteractionMethods> logger, IElementChainExecutor elementEvaluator, MethodContext methodContext)
            : base(browser, config, logger, elementEvaluator, methodContext)
        {
        }

        [InteractionMethod("select", Documentation = @"
            
            Select elements from the current page, using a CSS selector. For example:
   
            ```
            # All inputs with the attribute 'button'.
            select('input[type=button]') 
                
            # All elements with the class 'cssclass'.
            select('.cssclass')
            ```
        ")]
        public void Select(string selector)
        {
            AddToChain(q => q.Select(selector));
        }

        [InteractionMethod("withAttribute")]
        public void WithAttribute(string attributeName, string attributeValue)
        {
            AddToChain(q => q.WithAttribute(attributeName, attributeValue));
        }

        [InteractionMethod("withText")]
        public void WithText(string text)
        {
            AddToChain(q => q.WithText(text));
        }

        [InteractionMethod("displayed")]
        public void Displayed()
        {
            AddToChain(q => q.Displayed());
        }

        [InteractionMethod("assertAttribute", Documentation = @"
            Asserts that a named attribute on each element has a specific value. For example:

            ```
            # Verify the text attribute
            assertAttribute('text', 'Lorem Ipsum')
            
            # Verify the class attribute
            assertAttribute('class', 'enabled-highlight')
            ```
        ")]
        public async ValueTask AssertAttribute(string attributeName, string attributeValue, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertAttribute(attributeName, attributeValue));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("click")]
        public async ValueTask Click(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.Click());

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("clearInput", Documentation = @"

            Empties the contents of an input element.            

        ")]
        public async ValueTask ClearInput(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AddNode(nameof(ClearInput), (elements, browser) =>
            {
                var jsExec = (IJavaScriptExecutor)WebDriver;

                foreach (var element in elements)
                {
                    jsExec.ExecuteScript("arguments[0].select()", element);
                }
            }));
        }

        [InteractionMethod("type")]
        public async ValueTask Type(string text, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.Type(text));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertOne", Documentation = @"

            Assert that exactly one element exists in the chain. 

            If there are no elements, or more than one element is present, an error will be raised.

        ")]
        public async ValueTask AssertOne(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertSingle());

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertAtLeastOne")]
        public async ValueTask AssertAtLeastOne(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertAtLeast(1));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertText")]
        public async ValueTask AssertText(string text, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertAttribute("innerText", text));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }
    }
}
