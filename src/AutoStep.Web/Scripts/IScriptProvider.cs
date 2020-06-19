using System.Diagnostics.CodeAnalysis;

namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Defines a script provider, that can locate and provide modules.
    /// </summary>
    public interface IScriptProvider
    {
        /// <summary>
        /// Attempt to retrieve the content of a named module.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="moduleContent">The text of the module.</param>
        /// <returns>True if the module was available in the provider and content was loaded; false otherwise.</returns>
        public bool TryGetScriptModule(string moduleName, [NotNullWhen(true)] out string? moduleContent);

        /// <summary>
        /// Gets a description of the location the provider will look in for the named module (note that the module may not exist).
        /// </summary>
        /// <param name="moduleName">The name of the module to get a location description for.</param>
        /// <returns>The description.</returns>
        public string GetLocationDescription(string moduleName);
    }
}
