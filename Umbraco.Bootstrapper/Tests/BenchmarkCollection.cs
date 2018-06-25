using System;
using System.IO;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class BenchmarkCollection
    {

        public static object RunBenchmarks(string version, ConsoleArgs consoleArgs, Action globalSetupAction, Action globalCleanupAction)
        {
            var defaultConfig = new UmbracoDefaultConfig(version, consoleArgs, globalSetupAction, globalCleanupAction);

            //TODO: here's how we can run benchmarks for certain categories or filters https://github.com/dotnet/BenchmarkDotNet/issues/248
            // now we need to find a way to get a Summary back for each process and then combine those!
            // The summary part is here https://github.com/dotnet/BenchmarkDotNet/issues/305 ... not done yet
            // however, if we could somehow Pipe this Summary result back to the main process we can combine them but since a Summary isn't serializable
            // i don't think we can do it.

            var benchmarks = new[]
            {
                BenchmarkConverter.TypeToBenchmarks(typeof(ContentTypeOperations), defaultConfig),
                BenchmarkConverter.TypeToBenchmarks(typeof(ContentOperations), defaultConfig),
            };
            return BenchmarkRunner.Run(benchmarks);
        }
    }


}
