using System.IO;

namespace UmbracoBenchmarks.Tools
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
