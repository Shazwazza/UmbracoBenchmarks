using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Running;
using System;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using System.Linq;
using BenchmarkDotNet.Attributes.Jobs;

namespace UmbracoBenchmarks.Infrastructure
{
    [ShortRunJob]
    public class TestBenchmark
    {
        [Benchmark]
        public void DoThis()
        {
            Console.WriteLine("........ Doing it");
            Thread.Sleep(200);
        }

        [Benchmark]
        public void DoThat()
        {
            Console.WriteLine("........ Doing that");
            Thread.Sleep(400);
        }
    }

    public class AllowNonOptimized : ManualConfig
    {
        public AllowNonOptimized()
        {
            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
            Add(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default
        }
    }

    public class UmbracoBenchmarkToolchain : Toolchain
    {    
        public UmbracoBenchmarkToolchain(string exe) 
            : base("Umbraco", new InProcessGenerator(), new InProcessBuilder(), new UmbracoBenchmarkExecutor(exe))
        {
        }
        
    }
}
