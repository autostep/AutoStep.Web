using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public class InteractionMethods : BaseWebMethods
    {
        public InteractionMethods(IBrowser browser, ILogger<BaseWebMethods> logger, IElementChainExecutor elementEvaluator, MethodContext methodContext)
            : base(browser, logger, elementEvaluator, methodContext)
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

        [InteractionMethod("assertAttribute")]
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

        [InteractionMethod("type")]
        public async ValueTask Type(string text, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.Type(text));

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
