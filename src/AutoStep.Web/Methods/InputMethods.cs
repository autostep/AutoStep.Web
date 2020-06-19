using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;

namespace AutoStep.Web.Methods
{
    /// <summary>
    /// Defines interaction methods for working with input fields.
    /// </summary>
    public class InputMethods : BaseWebMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputMethods"/> class.
        /// </summary>
        /// <param name="dependencies">The set of dependencies for web methods.</param>
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
