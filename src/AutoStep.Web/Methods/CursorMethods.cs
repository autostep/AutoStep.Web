using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;

namespace AutoStep.Web.Methods
{
    /// <summary>
    /// Defines interaction methods for using the cursor.
    /// </summary>
    public class CursorMethods : BaseWebMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CursorMethods"/> class.
        /// </summary>
        /// <param name="dependencies">The set of dependencies.</param>
        public CursorMethods(IWebMethodServices<CursorMethods> dependencies)
            : base(dependencies)
        {
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Interaction method docs comes from an attribute; don't want to duplicate info.

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
    }
}
