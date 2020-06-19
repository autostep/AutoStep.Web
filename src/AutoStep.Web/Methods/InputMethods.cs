using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web.Methods
{
    public class InputMethods : BaseWebMethods
    {
        public InputMethods(
            IWebMethodServices<InputMethods> dependencies)
            : base(dependencies)
        {
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Interaction method docs comes from an attribute; don't want to duplicate info.

        [InteractionMethod("clearInput", Documentation = @"

            Empties the contents of all elements in the current set. For example:

            ```
            select('input[type=text]') -> clearInput()
            ```

            This method sets the 'value' attribute of each element to an empty string.

        ")]
        public async ValueTask ClearInput(CancellationToken cancelToken)
        {
            var chain = AddToChain(q => q.InvokeJavascript(ScriptRunner, "fields", "clearInputs"));

            // Concrete method; execute chain.
            await ExecuteChainAsync(chain, cancelToken);
        }
    }
}
