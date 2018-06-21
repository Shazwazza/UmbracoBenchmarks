using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Bootstrapper
{

    public class ConfigurationHelper
    {
        public static string GetEmbeddedDatabaseConnectionString()
        {
            return @"Data Source=|DataDirectory|\Umbraco.sdf;Flush Interval=1;";
        }

        public static Configuration GetConfig(string umbracoFolder)
        {
            var configFile = new FileInfo(Path.Combine(umbracoFolder, "web.config"));
            var configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile.FullName
            };
            return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        public static void CopyAppSettings(Configuration config)
        {
            foreach (var setting in config.AppSettings.Settings.Cast<KeyValueConfigurationElement>())
            {
                ConfigurationManager.AppSettings.Set(setting.Key, setting.Value);
            }
        }

        public static void CopyConnectionStrings(Configuration config, string dataDir)
        {
            //Important so things like SQLCE works
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);

            //Hack to be able to set configuration strings at runtime, needs reflection due to how MS built it
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var readonlyField = typeof(ConfigurationElementCollection).GetField("bReadOnly", flags);
            readonlyField.SetValue(ConfigurationManager.ConnectionStrings, false);

            foreach (var connectionString in config.ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>())
            {
                ConfigurationManager.ConnectionStrings.Add(connectionString);
            }

            readonlyField.SetValue(ConfigurationManager.ConnectionStrings, true);
        }

        public static void SetUmbracoConnectionString(string connString, string providerName)
        {
            //Hack to be able to set configuration strings at runtime, needs reflection due to how MS built it
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var readonlyField = typeof(ConfigurationElementCollection).GetField("bReadOnly", flags);
            readonlyField.SetValue(ConfigurationManager.ConnectionStrings, false);

            var isSet = false;
            foreach (var connectionString in ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>())
            {
                if (connectionString.Name == "umbracoDbDSN")
                {
                    connectionString.ConnectionString = connString;
                    connectionString.ProviderName = providerName;
                    isSet = true;
                    break;
                }
            }

            if (!isSet)
                throw new InvalidOperationException("No umbraco connection string found");

            readonlyField.SetValue(ConfigurationManager.ConnectionStrings, true);
        }
    }
}
