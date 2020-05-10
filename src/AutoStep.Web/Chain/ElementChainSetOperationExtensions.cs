using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Extension methods for manipulating the set of elements.
    /// </summary>
    public static class ElementChainSetOperationExtensions
    {
        /// <summary>
        /// Union a set of nested element chains together into one distinct list.
        /// </summary>
        /// <param name="chain">The parent chain.</param>
        /// <param name="buildCallbacks">A set of build callbacks for creating the nested chains.</param>
        /// <returns>An updated element chain.</returns>
        public static IElementChain Union(this IElementChain chain, params Func<IElementChain, IElementChain>[] buildCallbacks)
        {
            if (chain is null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

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
