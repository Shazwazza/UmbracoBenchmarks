using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static UmbracoBenchmarks.Md5VsSha256;
using System.Xml.XPath;

namespace UmbracoBenchmarks
{
    public class Runner
    {
        public void Run(string currDir, IEnumerable<ConfigVersion> configVersions)
        {
            if (string.IsNullOrWhiteSpace(currDir))
                throw new ArgumentException("message", nameof(currDir));

            var dlDir = Path.Combine(currDir, "UmbracoVersions");            
            if (!Directory.Exists(dlDir)) throw new InvalidOperationException($"The folder {dlDir} doesn't exist");
            var runnerDir = Path.Combine(currDir, "Runners");
            if (!Directory.Exists(dlDir)) throw new InvalidOperationException($"The folder {runnerDir} doesn't exist");

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                
                foreach (var versionConfig in configVersions)
                {
                    var runnerExe = Path.Combine(runnerDir, versionConfig.Runner);
                    if (!File.Exists(runnerExe)) throw new InvalidOperationException($"The file {runnerExe} doesn't exist");

                    var umbracoFolder = Path.Combine(dlDir, versionConfig.Version, "e");
                    if (!Directory.Exists(umbracoFolder)) throw new InvalidOperationException($"The folder {umbracoFolder} doesn't exist");
                    
                    CleanupUmbracoFolder(umbracoFolder);
                    var umbracoRunnerExe = CopyRunnerFiles(umbracoFolder, runnerExe);
                    //AddConfigTransforms(umbracoFolder, runnerExe);

                    using (Process process = new Process())
                    {
                        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(umbracoRunnerExe, $"{umbracoFolder} {versionConfig.Version}")
                        {
                            UseShellExecute = false,
                            //WorkingDirectory = sourceFolder
                            WorkingDirectory = Path.GetDirectoryName(umbracoRunnerExe)
                        };
                        process.StartInfo = myProcessStartInfo;
                        if (!process.Start())
                            break; //don't iterate if a process dies
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                            break; //don't iterate if a process dies
                    }
                }
            }
            finally
            {
                Console.ResetColor();
            }
            
        }

        /// <summary>
        /// Copy the runner exe and associated UmbracoBenchmark files to the source umbraco /bin folder to ensure that the 
        /// runner is executing with the correct umbraco version files.
        /// </summary>
        /// <param name="umbracoFolder"></param>
        /// <returns>The runner exe in the umbraco folder</returns>
        private string CopyRunnerFiles(string umbracoFolder, string runnerExe)
        {
            var umbracoRunnerExe = Path.Combine(umbracoFolder, "bin", Path.GetFileName(runnerExe));
            File.Copy(runnerExe, umbracoRunnerExe, true);

            //copy the other UmbracoBenchmark and required files too

            var files = new[] {
                "UmbracoBenchmarks.*",
                "BenchmarkDotNet.*", 
                "System.Threading.Tasks.Extensions.*", 
                "System.Collections.Immutable.*",
                "Microsoft.CodeAnalysis.*",
                "Microsoft.DotNet.PlatformAbstractions.*",
                "System.Reflection.*",
                "System.IO.FileSystem.*",
                "System.Runtime.InteropServices.*",
                "System.Security.Cryptography.*",
            };

            foreach (var file in files)
            {
                foreach (var f in Directory.EnumerateFiles(Path.GetDirectoryName(runnerExe), file, SearchOption.TopDirectoryOnly))
                {
                    File.Copy(f, Path.Combine(umbracoFolder, "bin", Path.GetFileName(f)), true);
                }
            }

            return umbracoRunnerExe;
        }

        private void AddConfigTransforms(string umbracoFolder, string runnerExe)
        {
            var sourceConfigFile = Path.Combine(Path.GetDirectoryName(runnerExe), Path.GetFileName(runnerExe) + ".config");
            if (!File.Exists(sourceConfigFile)) throw new InvalidOperationException($"The file {sourceConfigFile} was not found");
            var webConfigFile = Path.Combine(umbracoFolder, "web.config");
            if (!File.Exists(webConfigFile)) throw new InvalidOperationException($"The file {webConfigFile } was not found");
            
            XDocument webConfig;
            using (var reader = File.OpenText(webConfigFile))
            {
                webConfig = XDocument.Load(reader);
            }

            XDocument sourceConfig;
            using (var reader = File.OpenText(sourceConfigFile))
            {
                sourceConfig = XDocument.Load(reader);
            }

            var redirects = new[] { "System.Collections.Immutable" };

            var assemblyBindingRoot = webConfig.XPathSelectElement(GetAssemblyBindingRoot());
            if (assemblyBindingRoot == null) throw new InvalidOperationException("No assembly binding root found");

            foreach (var r in redirects)
            {
                var sourceRedirect = sourceConfig.XPathSelectElement(GetBindingXPath(r));
                if (sourceRedirect == null) throw new InvalidOperationException($"No XML element found for assembly {r}");

                var destRedirect = webConfig.XPathSelectElement(GetBindingXPath(r));
                if (destRedirect != null)
                {
                    //remove it so we can re-add it
                    destRedirect.Remove();
                }

                assemblyBindingRoot.Add(sourceRedirect);
            }

            webConfig.Save(webConfigFile);
        }

        /// <summary>
        /// Ugly xpath so we don't have to worry about xml namespaces
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private string GetBindingXPath(string assemblyName)
        {
            return $"/*[name()='configuration']/*[name()='runtime']/*[name()='assemblyBinding']//*[name()='dependentAssembly' and ./*[name()='assemblyIdentity'][@name='{assemblyName}']]";
        }

        /// <summary>
        /// Ugly xpath so we don't have to worry about xml namespaces
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyBindingRoot()
        {
            return $"/*[name()='configuration']/*[name()='runtime']/*[name()='assemblyBinding']";
        }

        private void CleanupUmbracoFolder(string umbracoFolder)
        {
            var appPlugins = Path.Combine(umbracoFolder, "App_Plugins");
            if (Directory.Exists(appPlugins))
                Directory.Delete(appPlugins, true);
            var appData = Path.Combine(umbracoFolder, "App_Data");
            if (Directory.Exists(appData))
                Directory.Delete(appData, true);
        }


    }
}
