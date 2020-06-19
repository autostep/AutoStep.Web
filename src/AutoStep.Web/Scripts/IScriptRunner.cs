namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Provides methods to invoke named script functions and methods within the browser.
    /// </summary>
    public interface IScriptRunner
    {
        /// <summary>
        /// Invoke a named function in a given module and returns the result.
        /// </summary>
        /// <param name="moduleName">
        /// The name of the module the function is located in.
        /// Typically this is the name of the file, minus any path or file extension.</param>
        /// <param name="functionName">
        /// The name of the function exported by <paramref name="moduleName"/> that should be invoked.
        /// If the module uses 'export default' to expose a single method, this value can be null.
        /// </param>
        /// <param name="args">The arguments to the JS function.</param>
        /// <returns>The return value of the function call.</returns>
        object? InvokeFunction(string moduleName, string? functionName, params object?[] args);

        /// <summary>
        /// Invoke a named function in a given module (without returning a result).
        /// </summary>
        /// <param name="moduleName">
        /// The name of the module the function is located in.
        /// Typically this is the name of the file,
        /// minus any path or file extension.</param>
        /// <param name="functionName">
        /// The name of the function exported by <paramref name="moduleName"/> that should be invoked.
        /// If the module uses 'export default' to expose a single method, this value can be null.
        /// </param>
        /// <param name="args">The arguments to the JS function.</param>
        void InvokeMethod(string moduleName, string? functionName, params object?[] args);
    }
}
