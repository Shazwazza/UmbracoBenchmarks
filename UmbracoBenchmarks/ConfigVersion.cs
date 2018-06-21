namespace UmbracoBenchmarks
{
    public class ConfigVersion
    {
        public ConfigVersion(string version, string source, string runner)
        {
            Version = version;
            Source = source;
            Runner = runner;
        }

        public string Version { get; }
        public string Source { get; }
        public string Runner { get; }
    }
}
