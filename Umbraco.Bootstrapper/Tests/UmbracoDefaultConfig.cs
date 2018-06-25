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
        public UmbracoDefaultConfig(string version, ConsoleArgs consoleArgs, Action globalSetupAction, Action globalCleanupAction)
        {
            ArtifactsPath = consoleArgs.ArtifactFolder.FullName;

            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());

            //csv exporter per version
            Add(CsvExporter.Default);
            //combined csv report per runId
            Add(new AppendingCsvExporter(consoleArgs.RunId));

            Set(new SummaryStyle { PrintUnitsInContent = false });

            GlobalSetupCallbacks.AddSetup(globalSetupAction);
            GlobalSetupCallbacks.AddCleanup(globalCleanupAction);

            var job = Job.ShortRun
                .WithLaunchCount(1)
                .With(RunStrategy.Monitoring)
                .With(InProcessToolchain.Instance)
                .WithId(version);

            Add(new TagColumn("VersionIndex", s => consoleArgs.Index.ToString()));

            Add(job);
        }
    }

}
