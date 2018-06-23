using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.Parameters;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Jobs;

namespace UmbracoBenchmarks
{
    public class UmbracoBenchmarkExecutor : IExecutor
    {
        private readonly string exe;

        public UmbracoBenchmarkExecutor(string exe)
        {
            this.exe = exe;
        }

        public ExecuteResult Execute(ExecuteParameters executeParameters)
        {
             //{umbracoFolder} {versionConfig.Version} {runId}

            var executor = new Executor();
            var artPaths = executeParameters.BuildResult.ArtifactsPaths;
            var umbArtPaths = new ArtifactsPaths(
                            null, //artPaths.RootArtifactsFolderPath,
                            null, //artPaths.BuildArtifactsDirectoryPath,
                            null, //artPaths.BinariesDirectoryPath,
                            null, //artPaths.ProgramCodePath,
                            null, //artPaths.AppConfigPath,
                            null, //artPaths.NuGetConfigPath,
                            null, //artPaths.ProjectFilePath,
                            null, //artPaths.BuildScriptFilePath,

                            //artPaths.ExecutablePath,
                            exe,

                            null);// artPaths.ProgramName);

            var umbBuildResults = BuildResult.Success(GenerateResult.Success(umbArtPaths, null));

            return executor.Execute(
                new ExecuteParameters(
                    umbBuildResults,
                    executeParameters.Benchmark,
                    executeParameters.Logger,
                    executeParameters.Resolver,
                    executeParameters.Config, 
                    executeParameters.Diagnoser));
        }
    }
}
