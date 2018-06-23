using System;
using System.IO;
using Umbraco.Core.IO;
using UmbracoBenchmarks.Tools;
using Umbraco.Core;
using Umbraco.Core.Configuration;

namespace UmbracoBenchmarks._77
{
    //STOLEN FROM https://github.com/Shazwazza/UmbracoLinqPadDriver

    public class ConsoleApplication : UmbracoApplicationBase
    {
        private readonly DirectoryInfo _umbracoFolder;

        public ConsoleApplication(DirectoryInfo umbracoFolder)
        {
            _umbracoFolder = umbracoFolder;
        }

        public void StartApplication()
        {
            //Now boot
            GetBootManager()
                .Initialize()
                .Startup(appContext => OnApplicationStarting(this, new EventArgs()))
                .Complete(appContext => OnApplicationStarted(this, new EventArgs()));
        }

        public ApplicationContext ApplicationContext => ApplicationContext.Current;

        protected override IBootManager GetBootManager()
        {
            return new ConsoleBootManager(this, _umbracoFolder);
        }

    }
}
