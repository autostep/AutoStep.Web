namespace AutoStep.Web.Scripts
{
    public interface IScriptRunner
    {
        object InvokeFunction(string moduleName, string? functionName, params object?[] args);

        void InvokeMethod(string moduleName, string? functionName, params object?[] args);
    }
}
