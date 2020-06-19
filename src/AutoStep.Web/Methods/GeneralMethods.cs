using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;

namespace AutoStep.Web.Methods
{
    /// <summary>
    /// Provides general interfaction methods for web elements (selecting, filtering, etc).
    /// </summary>
    public class GeneralMethods : BaseWebMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralMethods"/> class.
        /// </summary>
        /// <param name="dependencies">The web method dependencies.</param>
        public GeneralMethods(IWebMethodServices<InputMethods> dependencies)
            : base(dependencies)
        {
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

        [InteractionMethod("closestParent", Documentation = @"

            Finds the closest parent to each element in the set that matches a given CSS selector, traversing up the 
            elements in the page to the root element.

            Elements with no parent matching the selector are excluded from the results.

            For example, given the following HTML:

            ```html
            <div class=""first"">
                <div class=""second"">
                    <div class=""third"">Hello World</div>
                </div>
            </div>  
            ```

            Then the following chain would output the 'first' div:

            ```
            select('.third') -> closestParent('.first')
            ```
            
            but this chain would result in no elements:

            ```
            select('.second') -> closestParent('.third')
            ```
        ")]
        public void ClosestParent(string selector)
        {
            AddToChain(q => q.InvokeJavascript(ScriptRunner, "elements", "closestParent", selector));
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

        [InteractionMethod("withClass", Documentation = @"

            Filters the existing set of elements to those with a given class in their class list. For example:
 
            ```
            withClass('class-name')
            ```

        ")]
        public void WithClass(string className)
        {
            AddToChain(q => q.WithClass(className));
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

        [InteractionMethod("assertEnabled", Documentation = @"

            Assert that all the elements in the set are 'enabled', i.e. do not have the disabled attribute.

            If any of the elements in the set are disabled, a failure will be reported.

        ")]
        public async ValueTask AssertEnabled(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertEnabled());

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertDisabled", Documentation = @"

            Assert that all the elements in the set are 'disabled', i.e. have the disabled attribute.

            If any of the elements in the set are enabled, a failure will be reported.

        ")]
        public async ValueTask AssertDisabled(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertDisabled());

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertHasClass", Documentation = @"

            Assert that all the elements in the set have the given class name in their class list.

            If any of the elements in the set do not possess that class, an error will be raised.

        ")]
        public async ValueTask AssertHasClass(string className, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertHasClass(className));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }

        [InteractionMethod("assertDoesNotHaveClass", Documentation = @"

            Assert that all the elements in the set do not have the given class name in their class list.

            If any of the elements in the set do possess that class, an error will be raised.

        ")]
        public async ValueTask AssertDoesNotHaveClass(string className, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertDoesNotHaveClass(className));

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

        [InteractionMethod("assertTextIsNot", Documentation = @"
    
            Asserts that none of the elements in the current set have the specified text. For example:

            ```
            select('title') -> assertTextIsNot('Bad Title')
            ```
           
            This method uses the trimmed innerText of each element. It's not suitable for asserting
            the value of a given element.

        ")]
        public async ValueTask AssertTextIsNot(string text, CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.AssertTextIsNot(text));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }
    }
}
