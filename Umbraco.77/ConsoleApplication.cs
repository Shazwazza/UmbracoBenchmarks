using System;
using System.IO;
using Umbraco.Core.IO;
using Umbraco.Bootstrapper;
using Umbraco.Core;

namespace Umbraco._77
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
            //pre-init
            typeof(IOHelper).CallStaticMethod("SetRootDirectory", _umbracoFolder.FullName);

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
