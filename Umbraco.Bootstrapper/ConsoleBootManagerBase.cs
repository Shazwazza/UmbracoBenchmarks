using System;
using System.Configuration;
using System.IO;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.IO;

namespace UmbracoBenchmarks.Tools
{
    /// <summary>
    /// A shared boot manager that version specific boot managers override
    /// </summary>
    public abstract class ConsoleBootManagerBase : CoreBootManager
    {
        public DirectoryInfo UmbracoFolder { get; }
        public Configuration Config { get; }

        public ConsoleBootManagerBase(UmbracoApplicationBase umbracoApplication, DirectoryInfo umbracoFolder) : base(umbracoApplication)
        {
            UmbracoFolder = umbracoFolder;
            Config = ConfigurationHelper.GetConfig(UmbracoFolder.FullName);
        }

        /// <summary>
        /// Fires first in the application startup process before any customizations can occur
        /// </summary>
        /// <returns/>
        public override IBootManager Initialize()
        {   
            ConfigurationHelper.CopyConnectionStrings(Config, Path.Combine(UmbracoFolder.FullName, "App_Data"));
            //Use SQLCE (for now)
            ConfigurationHelper.SetUmbracoConnectionString(
                ConfigurationHelper.GetEmbeddedDatabaseConnectionString(),
                "System.Data.SqlServerCe.4.0");
            ConfigurationHelper.CopyAppSettings(Config);

            //Few folders that need to exist
            Directory.CreateDirectory(IOHelper.MapPath("~/App_Plugins"));

            return base.Initialize();
        }

        public override IBootManager Complete(Action<ApplicationContext> afterComplete)
        {
            var result = base.Complete(afterComplete);
            Console.WriteLine($"Umbraco version {UmbracoVersion.Current} started from folder {typeof(ApplicationContext).Assembly.CodeBase}");
            return result;
        }

        
    }
}
