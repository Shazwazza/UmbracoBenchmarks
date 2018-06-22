using System;
using Umbraco.Core;
using Umbraco.Core.IO;

namespace UmbracoBenchmarks.Tools
{

    /// <summary>
    /// Shared reflection utilities for Umbraco
    /// </summary>
    /// <remarks>
    /// Used so that there is less code duplication between the version specific boot strappers
    /// </remarks>
    public static class UmbracoUtilities
    {
        public static void SetIOHelperRoot(string umbracoFolder)
        {
            typeof(IOHelper).CallStaticMethod("SetRootDirectory", umbracoFolder);
        }

        public static void SetupDb(ApplicationContext appCtx)
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
