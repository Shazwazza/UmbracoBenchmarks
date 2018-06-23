using System;
using System.IO;

namespace UmbracoBenchmarks.Tools
{
    public class ConsoleArgs
    {
        public ConsoleArgs(DirectoryInfo umbracoFolder, string umbracoVersion, Guid runId)
        {
            UmbracoFolder = umbracoFolder;
            UmbracoVersion = umbracoVersion;
            RunId = runId;
        }

        public DirectoryInfo UmbracoFolder { get; }
        private DirectoryInfo _artifactFolder;
        public DirectoryInfo ArtifactFolder
        {
            get
            {
                if (_artifactFolder != null) return _artifactFolder;
                var folder = Path.Combine(UmbracoFolder.Parent.Parent.Parent.FullName, "BenchmarkDotNet.Artifacts", RunId.ToString());
                //Directory.CreateDirectory(folder);
                _artifactFolder = new DirectoryInfo(folder);
                return _artifactFolder;
            }
        }
        public string UmbracoVersion { get; }
        public Guid RunId { get; }
    }
}
