using System;
using System.IO;
using Umbraco.Bootstrapper;

namespace Umbraco._77
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var consoleArgs = ConsoleHelper.ParseArgs(args);
                ConsoleHelper.Setup(consoleArgs);
                ConsoleHelper.Cleanup(consoleArgs.UmbracoFolder);

                using (var app = new ConsoleApplication(consoleArgs.UmbracoFolder))
                {
                    app.StartApplication();
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
