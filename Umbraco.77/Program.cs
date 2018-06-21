using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Umbraco.Bootstrapper;
using Umbraco.Core;
using Umbraco.Core.Models;

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
                    SetupDb(app.ApplicationContext);
                    CreateContent(app.ApplicationContext);
                }

                Console.WriteLine("Done");
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void CreateContent(ApplicationContext appCtx)
        {
            Console.Write("Creating content... ");

            var dts = new[] { "Label", "Textstring", "Richtext editor" }.Select(x =>
            {
                var dt = appCtx.Services.DataTypeService.GetDataTypeDefinitionByName(x);
                if (dt == null) throw new InvalidOperationException($"No data type found by name {x}");
                return dt;
            }).ToList();
            
            var ct = new ContentType(-1)
            {
                Alias = "home",
                Name = "Home",
                AllowedAsRoot = true
            };
            ct.AddPropertyGroup("test1");
            ct.AddPropertyGroup("test2");
            ct.AddPropertyGroup("test3");
            ct.AddPropertyType(new PropertyType(dts[0], "test1"), "test1");
            ct.AddPropertyType(new PropertyType(dts[1], "test2"), "test2");
            ct.AddPropertyType(new PropertyType(dts[2], "test3"), "test3");
            appCtx.Services.ContentTypeService.Save(ct);

            Console.WriteLine("OK");
            Console.WriteLine($"Total content types: {appCtx.Services.ContentTypeService.GetAllContentTypes().Count()}");
        }

        private static void SetupDb(ApplicationContext appCtx)
        {
            Console.Write("Creating and installing DB... ");

            appCtx.DatabaseContext.ConfigureEmbeddedDatabaseConnection();
            var result = appCtx.DatabaseContext.CallMethod("CreateDatabaseSchemaAndData", appCtx);
            var success = (bool)result.GetPropertyValue("Success");
            
            Console.WriteLine(success ? "OK" : "Failed");
            
            if (!success)
            {
                Console.Write(result.GetPropertyValue("Message"));
            }
        }
    }
}
