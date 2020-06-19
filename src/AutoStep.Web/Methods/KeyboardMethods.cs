using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;

namespace AutoStep.Web.Methods
{
    public class KeyboardMethods : BaseWebMethods
    {
        public KeyboardMethods(IWebMethodServices<CursorMethods> dependencies)
            : base(dependencies)
        {
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Interaction method docs comes from an attribute; don't want to duplicate info.

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
    }
}
