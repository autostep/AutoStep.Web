using AutoStep.Web.Chain;

namespace AutoStep.Web.Methods
{
    /// <summary>
    /// Defines interaction methods for interacting with buttons.
    /// </summary>
    public class ButtonMethods : BaseWebMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonMethods"/> class.
        /// </summary>
        /// <param name="dependencies">The web method dependencies.</param>
        public ButtonMethods(IWebMethodServices<ButtonMethods> dependencies)
            : base(dependencies)
        {
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Interaction method docs comes from an attribute; don't want to duplicate info.

        [InteractionMethod("locateButtonWithDefaultRules", Documentation = @"

            Locates a button by it's name using a standard set of rules.

            These rules are:

            1. Any `<button>` element with the right text or an aria-label attribute:
               
               ```html
               <button type=""submit"">Button Name</button>
               <!-- or --> 
               <button type=""submit"" aria-label=""Button Name""><i class=""fas fa-mouse"" /></button>
               ```

            2. Any element with the `role='button'` attribute, with the right text or an aria-label attribute:
               
               ```html
               <a role=""button"">Button Name</a>
               <!-- or --> 
               <a role=""button"" aria-label=""Button Name""><i class=""fas fa-mouse"" /></a>
               ```

            3. Any `input` of type `button`, `submit` or `reset` that has a matching value
               attribute or an aria-label attribute:               

               ```html
               <input type=""submit"">Button Name</button>
               <!-- or --> 
               <input type=""button"">Button Name</button>
               ```
        ")]
        public void LocateButtonByStandardRules(string name)
        {
            AddToChain(c => c.Union(
                c => c.Select("button,[role=button]")
                      .Union(
                        c => c.WithAttribute("aria-label", name),
                        c => c.WithText(name)),
                c => c.Select("input[type=button],input[type=submit],input[type=reset]")
                      .Union(
                        c => c.WithAttribute("value", name),
                        c => c.WithAttribute("aria-label", name))));
        }
    }
}
