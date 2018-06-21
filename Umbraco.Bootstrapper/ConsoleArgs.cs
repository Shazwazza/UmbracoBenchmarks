using System.IO;

namespace Umbraco.Bootstrapper
{
    public class ConsoleArgs
    {
        public ConsoleArgs(DirectoryInfo umbracoFolder, string umbracoVersion)
        {
            UmbracoFolder = umbracoFolder;
            UmbracoVersion = umbracoVersion;
        }

        public DirectoryInfo UmbracoFolder { get; }
        public string UmbracoVersion { get; }
    }
}
