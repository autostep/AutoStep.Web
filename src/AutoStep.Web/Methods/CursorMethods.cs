using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;

namespace AutoStep.Web.Methods
{
    public class CursorMethods : BaseWebMethods
    {
        public CursorMethods(IWebMethodServices<CursorMethods> dependencies)
            : base(dependencies)
        {
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
    }
}
