using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;
using System.Linq;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class UmbracoDefaultConfig : ManualConfig
    {
        public UmbracoDefaultConfig(string version)
        {
            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
            Add(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default

            Add(new TagColumn("Ver", name => version));

            Add(Job.ShortRun
                .WithLaunchCount(1)
                .With(RunStrategy.Monitoring)
                .With(InProcessToolchain.Instance)
                .WithId("InProcess"));
        }
    }

}
