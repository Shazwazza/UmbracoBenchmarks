using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;
using System;
using System.IO;
using System.Linq;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class UmbracoDefaultConfig : ManualConfig
    {
        public UmbracoDefaultConfig(string version, Guid runId, DirectoryInfo artifactFolder, Action globalSetupAction, Action globalCleanupAction)
        {
            ArtifactsPath = artifactFolder.FullName;

            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());

            //csv exporter per version
            Add(CsvExporter.Default);
            //combined csv report per runId
            Add(new AppendingCsvExporter(runId));

            Set(new SummaryStyle { PrintUnitsInContent = false });

            GlobalSetupCallbacks.AddSetup(globalSetupAction);
            GlobalSetupCallbacks.AddCleanup(globalCleanupAction);

            var job = Job.ShortRun
                .WithLaunchCount(1)
                .With(RunStrategy.Monitoring)
                .With(InProcessToolchain.Instance)
                .WithId(version);

            Add(job);
        }
    }

}
