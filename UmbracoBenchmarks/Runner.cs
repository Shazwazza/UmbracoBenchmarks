using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                    var sourceFolder = Path.Combine(dlDir, versionConfig.Version, "e");
                    if (!Directory.Exists(sourceFolder)) throw new InvalidOperationException($"The folder {sourceFolder} doesn't exist");

                    using (Process process = new Process())
                    {
                        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(runnerExe, $"{sourceFolder} {versionConfig.Version}")
                        {
                            UseShellExecute = false,
                            //WorkingDirectory = sourceFolder
                            WorkingDirectory = Path.GetDirectoryName(runnerExe)
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

        
    }
}
