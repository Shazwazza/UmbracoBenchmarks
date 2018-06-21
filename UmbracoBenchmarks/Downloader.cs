using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UmbracoBenchmarks
{

    public class Downloader : IDisposable
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task Download(string currDir, IEnumerable<ConfigVersion> configVersions)
        {
            if (string.IsNullOrWhiteSpace(currDir))
                throw new ArgumentException("message", nameof(currDir));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Downloading specified Umbraco versions...");
            
            var dlDir = Path.Combine(currDir, "UmbracoVersions");
            Directory.CreateDirectory(dlDir);
            foreach (var v in configVersions)
            {
                var versionDir = Path.Combine(dlDir, v.Version);
                Directory.CreateDirectory(versionDir);
                var sourceZip = Path.Combine(versionDir, "source.zip");
                var exists = File.Exists(sourceZip);

                Console.WriteLine(exists ? $"local copy of {v.Version} already exists" : $"downloading zip for {v.Version}...");

                if (!exists)
                {
                    var stream = await _httpClient.GetStreamAsync(v.Source);
                    using (var newFile = File.Create(sourceZip))
                    {
                        await stream.CopyToAsync(newFile);
                    }
                }

                var extractedDir = Path.Combine(versionDir, "e");
                var binExists = Directory.Exists(Path.Combine(extractedDir, "bin"));

                Console.WriteLine(binExists ? $"unzipped copy of {v.Version} already exists" : $"unzipping version {v.Version}...");

                if (!binExists)
                {
                    ZipFile.ExtractToDirectory(sourceZip, extractedDir);
                }
            }
        }

        public void Dispose()
        {
            Console.ResetColor();
            _httpClient.Dispose();
        }
    }
}
