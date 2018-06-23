using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UmbracoBenchmarks.Infrastructure
{
    public class UmbracoBenchmarkConfig : ManualConfig
    {
        public UmbracoBenchmarkConfig(string dlDir, string runnerDir, IEnumerable<ConfigVersion> configVersions, Guid runId)
        {
            ArtifactsPath = GetArtifactFolder(new DirectoryInfo(dlDir), runId);

            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray());

            //Add(DefaultConfig.Instance.GetExporters().ToArray());
            Add(CsvExporter.Default);

            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());

            Add(CsvMeasurementsExporter.Default);

            foreach (var versionConfig in configVersions)
            {
                var runnerExe = Path.Combine(runnerDir, versionConfig.Runner);
                if (!File.Exists(runnerExe)) throw new InvalidOperationException($"The file {runnerExe} doesn't exist");

                var umbracoFolder = Path.Combine(dlDir, versionConfig.Version, "e");
                if (!Directory.Exists(umbracoFolder)) throw new InvalidOperationException($"The folder {umbracoFolder} doesn't exist");

                Add(Job.ShortRun
                    .WithLaunchCount(1)
                    .With(RunStrategy.Monitoring)
                    .With(new UmbracoBenchmarkToolchain(runnerExe))
                    .WithId(versionConfig.Version));
            }

            //Add(new TagColumn("Ver", name => version));


            //Add(InProcessValidator.DontFailOnError);
        }

        private string GetArtifactFolder(DirectoryInfo dlDir, Guid runId)
        {
            var folder = Path.Combine(dlDir.Parent.FullName, "BenchmarkDotNet.Artifacts", runId.ToString());
            return folder;
        }
    }
}
