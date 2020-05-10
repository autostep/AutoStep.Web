namespace AutoStep.Web.Scripts
{
    public interface IScriptProvider
    {
        public bool TryGetScriptModule(string moduleName, out string? moduleContent);

        public string GetLocationDescription(string moduleName);
    }
}
