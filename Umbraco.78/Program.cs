using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using UmbracoBenchmarks.Tools;
using Umbraco.Core;
using Umbraco.Core.Models;
using UmbracoBenchmarks.Tools.Tests;
using Umbraco.Core.Configuration;

namespace UmbracoBenchmarks._78
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleArgs = ConsoleHelper.ParseArgs(args);
            var result = BenchmarkCollection.RunBenchmarks(
                UmbracoVersion.Current.ToString(),
                consoleArgs.RunId,
                consoleArgs.ArtifactFolder,
                () => StartUmbraco(consoleArgs),
                ShutdownUmbraco);
        }

        private static ConsoleApplication _consoleApp;
        private static volatile bool _setupRun = false;
        private static void StartUmbraco(ConsoleArgs consoleArgs)
        {
            if (_setupRun) return;
            _setupRun = true;

            Console.WriteLine("START UMBRACO");
            _consoleApp = new ConsoleApplication(consoleArgs.UmbracoFolder);
            _consoleApp.StartApplication();
            UmbracoUtilities.SetupDb(_consoleApp.ApplicationContext);
        }

        private static void ShutdownUmbraco()
        {
            Console.WriteLine("SHUTDOWN UMBRACO");
        }
    }
}
