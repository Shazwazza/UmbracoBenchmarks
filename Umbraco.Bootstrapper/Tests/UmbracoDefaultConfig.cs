using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;
using System;
using System.IO;
using System.Linq;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class UmbracoDefaultConfig : ManualConfig
    {
        public UmbracoDefaultConfig(string version, DirectoryInfo artifactFolder, Action globalSetupAction, Action globalCleanupAction)
        {
            ArtifactsPath = artifactFolder.FullName;

            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            
            //Add(DefaultConfig.Instance.GetExporters().ToArray());
            Add(CsvExporter.Default);
            
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            
            Add(CsvMeasurementsExporter.Default);

            //Add(new TagColumn("Ver", name => version));

            GlobalSetupCallbacks.AddSetup(globalSetupAction);
            GlobalSetupCallbacks.AddCleanup(globalCleanupAction);

            var job = Job.ShortRun
                .WithLaunchCount(1)
                //.With((IEngineFactory)new UmbracoEngineFactory(globalSetupAction, globalCleanupAction))
                .With(RunStrategy.Monitoring)
                .With(InProcessToolchain.Instance)
                .WithId(version);

            Add(job);
        }
    }

}
