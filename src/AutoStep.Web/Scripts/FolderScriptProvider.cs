using System;
using System.Collections.Concurrent;
using System.IO;
using AutoStep.Extensions;

namespace AutoStep.Web.Scripts
{
    public class FolderScriptProvider : IScriptProvider
    {
        private readonly string folder;
        private readonly string absoluteFolder;
        private readonly bool useMinifiedScripts;

        private ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        public FolderScriptProvider(IAutoStepEnvironment environment, string folder, bool useMinifiedScripts)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            this.folder = folder;

            if (Path.IsPathRooted(folder))
            {
                absoluteFolder = folder;
            }
            else
            {
                absoluteFolder = Path.GetFullPath(folder, environment.RootDirectory);
            }

            this.useMinifiedScripts = useMinifiedScripts;
        }

        public string GetLocationDescription(string moduleName)
        {
            if (useMinifiedScripts)
            {
                return string.Format("{0}.min.js or {0}.js in {1}.", moduleName, folder);
            }
            else
            {
                return string.Format("{0}.js in {1}.", moduleName, folder);
            }
        }

        public bool TryGetScriptModule(string moduleName, out string? moduleContent)
        {
            if (cache.TryGetValue(moduleName, out moduleContent))
            {
                return true;
            }

            // See if the file exists.
            if (useMinifiedScripts)
            {
                var minifiedPath = Path.Combine(absoluteFolder, moduleName + ".min.js");
                if (File.Exists(minifiedPath))
                {
                    moduleContent = File.ReadAllText(minifiedPath);
                    cache.TryAdd(moduleName, moduleContent);
                    return true;
                }
            }

            var fullPath = Path.Combine(absoluteFolder, moduleName + ".js");

            if (File.Exists(fullPath))
            {
                moduleContent = File.ReadAllText(fullPath);
                cache.TryAdd(moduleName, moduleContent);
                return true;
            }

            return false;
        }
    }
}
