using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class BenchmarkCollection
    {
        public static Summary RunBenchmarks(string version)
        {
            var defaultConfig = new UmbracoDefaultConfig(version);

            //TODO: here's how we can run benchmarks for certain categories or filters https://github.com/dotnet/BenchmarkDotNet/issues/248
            // now we need to find a way to get a Summary back for each process and then combine those!

            var benchmarks = new[] 
            { 
                BenchmarkConverter.TypeToBenchmarks(typeof(CreateContentTypeBenchmark), defaultConfig),
                BenchmarkConverter.TypeToBenchmarks(typeof(CreateContentBenchmark), defaultConfig),
            };
            return BenchmarkRunner.Run(benchmarks);
        }
    }


}
