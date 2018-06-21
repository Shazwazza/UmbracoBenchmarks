using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UmbracoBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting benchmark runner");
            
            var currDir = Directory.GetCurrentDirectory();
            var configVersions = new List<ConfigVersion>();
            using (var reader = File.OpenText(Path.Combine(currDir, "versions.xml")))
            {
                var xml = XDocument.Load(reader);
                foreach (var v in xml.Root.Elements("version"))
                {
                    configVersions.Add(new ConfigVersion((string)v.Attribute("value"), (string)v.Attribute("source"), (string)v.Attribute("runner")));
                }
            }
            using (var dl = new Downloader())
            {   
                dl.Download(currDir, configVersions).Wait();
            }

            var runner = new Runner();
            runner.Run(currDir, configVersions);

            Console.WriteLine("Done");
        }
    }
}
