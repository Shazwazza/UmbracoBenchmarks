using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbracoBenchmarks.Tools;

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

                    //TODO: The runner exe is what will be running and therefore what will be benchmarked so instead of doing a post build command
                    // on the runner exe projects, we need to manually copy out the runner files into corresponding /runner/{umbraco_version} folders
                    // then we need to copy the Umbraco DLLs from the umbraco version to the runner. This will mean that we are actually benchmarking/running
                    // the correct version DLLs and not what the runner was compiled with.

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
            //copy the other UmbracoBenchmark files too
            foreach(var f in Directory.EnumerateFiles(Path.GetDirectoryName(runnerExe), "UmbracoBenchmarks.*", SearchOption.TopDirectoryOnly))
            {
                File.Copy(f, Path.Combine(umbracoFolder, "bin", Path.GetFileName(f)), true);
            }
            return umbracoRunnerExe;
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
