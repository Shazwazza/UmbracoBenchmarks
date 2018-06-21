using System;
using System.IO;
using Umbraco.Bootstrapper;

namespace Umbraco._78
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

                    Console.Write("Creating and installing DB... ");
                    app.ApplicationContext.DatabaseContext.ConfigureEmbeddedDatabaseConnection();
                    var result = app.ApplicationContext.DatabaseContext.CallMethod("CreateDatabaseSchemaAndData", app.ApplicationContext);
                    var success = (bool)result.GetPropertyValue("Success");
                    Console.WriteLine(success ? "OK" : "Failed");
                    if (!success)
                    {
                        Console.Write(result.GetPropertyValue("Message"));
                    }
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
