using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using AutoStep.Extensions;

namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Defines a script provider that looks in a local folder for scripts.
    /// </summary>
    public class FolderScriptProvider : IScriptProvider
    {
        private const string MinifiedJsExtension = ".min.js";
        private const string JsExtension = ".js";

        private readonly char[] bannedModuleNameCharacters = new[]
        {
            '/',
            '\\',
        };

        private readonly string folder;
        private readonly string absoluteFolder;
        private readonly bool useMinifiedScripts;

        private ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderScriptProvider"/> class.
        /// </summary>
        /// <param name="environment">The autostep environment.</param>
        /// <param name="folder">
        /// The path to a folder to look for scripts in. If this is a relative path, it is relative to the AutoStep project
        /// root directory.
        /// </param>
        /// <param name="useMinifiedScripts">If true, then the provider will prefer minified scripts (*.min.js) over regular files.</param>
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

        /// <inheritdoc/>
        public string GetLocationDescription(string moduleName)
        {
            if (useMinifiedScripts)
            {
                return FolderScriptProviderMessages.LocationDescriptionUseMinified.FormatWith(moduleName, folder);
            }
            else
            {
                return FolderScriptProviderMessages.LocationDescriptionNoMinified.FormatWith(moduleName, folder);
            }
        }

        /// <inheritdoc/>
        public bool TryGetScriptModule(string moduleName, [NotNullWhen(true)] out string? moduleContent)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentException(Messages.ParameterCannotBeEmpty, nameof(moduleName));
            }

            if (cache.TryGetValue(moduleName, out moduleContent))
            {
                return true;
            }

            if (moduleName.IndexOfAny(bannedModuleNameCharacters) != -1)
            {
                throw new ArgumentException(FolderScriptProviderMessages.InvalidModuleNameChars.FormatWith(moduleName, string.Join(", ", bannedModuleNameCharacters)), nameof(moduleName));
            }

            // See if the file exists.
            if (useMinifiedScripts)
            {
                var minifiedPath = Path.Combine(absoluteFolder, moduleName + MinifiedJsExtension);
                if (File.Exists(minifiedPath))
                {
                    moduleContent = File.ReadAllText(minifiedPath);
                    cache.TryAdd(moduleName, moduleContent);
                    return true;
                }
            }

            var fullPath = Path.Combine(absoluteFolder, moduleName + JsExtension);

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
