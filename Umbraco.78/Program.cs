using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using UmbracoBenchmarks.Tools;
using Umbraco.Core;
using Umbraco.Core.Models;
using UmbracoBenchmarks.Tools.Tests;

namespace UmbracoBenchmarks._78
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
                    CreateContent.Execute(app.ApplicationContext);
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
