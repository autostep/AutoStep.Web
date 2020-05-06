using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoStep.Web.Chain
{
    public static class ElementChainSetOperationExtensions
    {
        public static IElementChain Union(this IElementChain chain, params Func<IElementChain, IElementChain>[] buildCallbacks)
        {
            return chain.AddGroupingNode(
                nameof(Union),
                resultSets =>
                {
                    // Concatenate all distinct elements.
                    return resultSets.SelectMany(x => x).Distinct().ToList();
                },
                buildCallbacks);
        }
    }
}
