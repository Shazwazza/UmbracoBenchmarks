using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using UmbracoBenchmarks.Tools;
using Umbraco.Core;
using Umbraco.Core.Models;
using UmbracoBenchmarks.Tools.Tests;
using Umbraco.Core.Configuration;

namespace UmbracoBenchmarks._77
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var consoleArgs = ConsoleHelper.ParseArgs(args);
                ConsoleHelper.Setup(consoleArgs);

                using (var app = new ConsoleApplication(consoleArgs.UmbracoFolder))
                {
                    app.StartApplication();
                    UmbracoUtilities.SetupDb(app.ApplicationContext);
                    var result = BenchmarkCollection.RunBenchmarks(UmbracoVersion.Current.ToString());
                }

                Console.WriteLine("Done");
            }
            finally
            {
                Console.ResetColor();
            }
        }
        
    }
}
