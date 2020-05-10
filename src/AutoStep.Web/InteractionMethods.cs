using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web
{
    /// <summary>
    /// Defines general element interaction methods.
    /// </summary>
    public sealed class InteractionMethods : BaseWebMethods
    {
        private readonly IScriptRunner scriptRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionMethods"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="chainExecutor">A chain executor.</param>
        /// <param name="scriptRunner">A script runner.</param>
        /// <param name="methodContext">The active method context.</param>
        public InteractionMethods(
            IBrowser browser,
            IConfiguration config,
            ILogger<InteractionMethods> logger,
            IElementChainExecutor chainExecutor,
            IScriptRunner scriptRunner,
            MethodContext methodContext)
            : base(browser, config, logger, chainExecutor, methodContext)
        {
            this.scriptRunner = scriptRunner;
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Interaction method docs comes from an attribute; don't want to duplicate info.

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

        [InteractionMethod("withAttribute", Documentation = @"

            Filters the existing set of elements to those with a given attribute value. For example:
 
            ```
            withAttribute('class', 'class attribute value')
            withAttribute('id', 'Element id')
            ```

        ")]
        public void WithAttribute(string attributeName, string attributeValue)
        {
            AddToChain(q => q.WithAttribute(attributeName, attributeValue));
        }

        [InteractionMethod("withText", Documentation = @"

            Filters the existing set of elements to those with the given text. For example:

            ```
            select('p') -> withText('Hello')
            ```

            This method uses the trimmed innerText of each element. It's not suitable for getting input
            elements with a given value.

        ")]
        public void WithText(string text)
        {
            AddToChain(q => q.WithText(text));
        }

        [InteractionMethod("displayed", Documentation = @"
            
            Filters the existing set of elements to those that are visible in the viewport. For example:

            ```
            # Outputs all buttons that are visible.
            select('button') -> displayed()
            ```

        ")]
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

        [InteractionMethod("click", Documentation = @"
    
            Clicks on the first element in the existing set.

            If the set is empty, or the element is not displayed, we'll raise an error.        

        ")]
        public async ValueTask Click(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.Click());

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("clearInput", Documentation = @"

            Empties the contents of all elements in the current set. For example:

            ```
            select('input[type=text]') -> clearInput()
            ```

            This method sets the 'value' attribute of each element to an empty string.

        ")]
        public async ValueTask ClearInput(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.InvokeJavascript(scriptRunner, "fields", "clearInputs"));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("type", Documentation = @"
                        
            Type characters onto the page, or into an element. For example:
            
            ```
            # Type onto the page.
            type('Some content')

            # Type into an element.
            select('input[type=text]') -> type('field value')
            ```

            This method has two different behaviours, depending on if there are any elements
            in the current set.
            
            1. If there are no elements in the set, then this method will just type directly onto the page,
               or into whichever element currently has focus.
            2. If there are elements in the set, this method will type into the first element; it will raise
               an error if the first element is not displayed, or is not enabled.

        ")]
        public async ValueTask Type(string text, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.Type(text));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertOne", Documentation = @"

            Assert that exactly one element exists in the current set. 

            If there are no elements, or more than one element is present, an error will be raised.

        ")]
        public async ValueTask AssertOne(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertSingle());

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertAtLeastOne", Documentation = @"

            Assert that at least one element exists in the current set.

            If there are no elements in the set, an error will be raised.
        
        ")]
        public async ValueTask AssertAtLeastOne(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertAtLeast(1));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertText", Documentation = @"
    
            Asserts that all the elements in the current set have the specified text. For example:

            ```
            select('title') -> assertText('My Title')
            ```
           
            This method uses the trimmed innerText of each element. It's not suitable for asserting
            the value of a given element.

        ")]
        public async ValueTask AssertText(string text, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertText(text));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }
    }
}
