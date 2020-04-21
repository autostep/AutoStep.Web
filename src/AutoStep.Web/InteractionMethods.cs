using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.ElementChain;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    public class InteractionMethods : BaseWebMethods
    {
        public InteractionMethods(IBrowser browser, ILogger<BaseWebMethods> logger, MethodContext methodContext)
            : base(browser, logger, methodContext)
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
            Chain(q => q.Select(selector));
        }

        [InteractionMethod("withAttribute")]
        public void WithAttribute(string attributeName, string attributeValue)
        {
            Chain(q => q.WithAttribute(attributeName, attributeValue));
        }

        [InteractionMethod("withText")]
        public void WithText(string text)
        {
            Chain(q => q.WithText(text));
        }

        [InteractionMethod("displayed")]
        public void Displayed()
        {
            Chain(q => q.Displayed());
        }

        [InteractionMethod("assertAttribute")]
        public async ValueTask AssertAttribute(string attributeName, string attributeValue, CancellationToken cancelToken)
        {
            var chain = Chain(q => q.AssertAttribute(attributeName, attributeValue));

            // Concrete step; evaluate the chain.
            await chain.EvaluateAsync(cancelToken);
        }

        [InteractionMethod("click")]
        public async ValueTask Click(CancellationToken cancelToken)
        {
            var chain = Chain(q => q.Click());

            // Concrete step; evaluate the chain.
            await chain.EvaluateAsync(cancelToken);
        }

        [InteractionMethod("type")]
        public async ValueTask Type(string text, CancellationToken cancelToken)
        {
            var chain = Chain(q => q.Type(text));

            // Concrete step; evaluate the chain.
            await chain.EvaluateAsync(cancelToken);
        }

        [InteractionMethod("assertAtLeastOne")]
        public async ValueTask AssertAtLeastOne(CancellationToken cancelToken)
        {
            var chain = Chain(q => q.AssertAtLeast(1));

            // Concrete step; evaluate the chain.
            await chain.EvaluateAsync(cancelToken);
        }

        [InteractionMethod("assertText")]
        public async ValueTask AssertText(string text, CancellationToken cancelToken)
        {
            var chain = Chain(q => q.AssertAttribute("innerText", text));

            // Concrete step; evaluate the chain.
            await chain.EvaluateAsync(cancelToken);
        }
    }
}
