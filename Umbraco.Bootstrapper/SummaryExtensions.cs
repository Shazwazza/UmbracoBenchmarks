using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UmbracoBenchmarks.Tools
{
    public static class SummaryExtensions
    {
        //TODO: Taken from Benchmark .NET source since their Join method is internal
        public static Summary Join(this List<Summary> summaries, IConfig commonSettingsConfig, ClockSpan clockSpan)
            => new Summary(
                $"BenchmarkRun-joined-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}",
                summaries.SelectMany(summary => summary.Reports).ToArray(),
                HostEnvironmentInfo.GetCurrent(),
                commonSettingsConfig,
                summaries.First().ResultsDirectoryPath,
                clockSpan.GetTimeSpan(),
                summaries.SelectMany(summary => summary.ValidationErrors).ToArray());
    }
}
