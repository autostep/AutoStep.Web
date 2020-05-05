using System;
using System.Collections.Generic;
using System.Text;

namespace AutoStep.Web.Chain
{
    public static class SetOperationExtensions
    {
        public static IElementChain Concat(this IElementChain chain, params Func<IElementChain, IElementChain>[] buildCallbacks)
        {
            return chain;
        }
    }
}
