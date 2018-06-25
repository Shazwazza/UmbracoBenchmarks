using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.IO;
using Umbraco.Core.ObjectResolution;
using UmbracoBenchmarks.Tools;

namespace UmbracoBenchmarks._75
{
    //STOLEN FROM https://github.com/Shazwazza/UmbracoLinqPadDriver

    public class ConsoleBootManager : ConsoleBootManagerBase
    {

        public ConsoleBootManager(UmbracoApplicationBase umbracoApplication, DirectoryInfo umbracoFolder)
            : base(umbracoApplication, umbracoFolder)
        {
            UmbracoUtilities.SetIOHelperRoot(umbracoFolder.FullName);
        }

        /// <summary>
        /// Fires first in the application startup process before any customizations can occur
        /// </summary>
        /// <returns/>
        public override IBootManager Initialize()
        {
            ConfigureUmbracoSettings(Config);
            return base.Initialize();
        }

        public override IBootManager Startup(Action<ApplicationContext> afterStartup)
        {
            ////This is a special case and is due to a timing issue, we need to wait until the appCtx singleton is created
            ////and then we can forcefully configure the file system providers since unfortunatley some of those rely on the singleton
            //ConfigureFileSystemProviders(Config);

            return base.Startup(afterStartup);
        }
        
        /// <summary>
        /// Disables all application level cache
        /// </summary>
        protected override CacheHelper CreateApplicationCache()
        {
            return CacheHelper.CreateDisabledCacheHelper();
        }

        /// <summary>
        /// The main problem with booting umbraco is all of the startup handlers that will not work without a web context or in a standalone
        /// mode. So this code removes all of those handlers. We of course need some of them so this attempts to just keep the startup handlers
        /// declared inside of Umbraco.Core.
        /// </summary>
        protected override void InitializeApplicationEventsResolver()
        {
            base.InitializeApplicationEventsResolver();

            var appEventsResolverType = typeof(ApplicationContext).Assembly.GetType("Umbraco.Core.ObjectResolution.ApplicationEventsResolver");
            var appEventsResolver = ReflectionHelper.CastTo(appEventsResolverType.GetStaticProperty("Current"), appEventsResolverType);
            //get the legacy resolver, unfortunately this needs reflection currently - this is because the core's RemoveType functionality
            //doesn't also filter the legacy ones which it needs to do 
            var legacyResolverType = typeof(ManyObjectsResolverBase<,>).MakeGenericType(appEventsResolverType, typeof(IApplicationStartupHandler));
            var legacyAppEventsResolver = ReflectionHelper.CastTo(
                ReflectionHelper.GetFieldValue(appEventsResolver, "_legacyResolver"),
                legacyResolverType);

            //now we want to get all IApplicationStartupHandlers from the PluginManager
            var startupHandlers = PluginManager.Current.ResolveTypes<IApplicationStartupHandler>();
            //for now we're just going to remove any type that does not exist in Umbraco.Core
            foreach (var startupHandler in startupHandlers
                .Where(x => x.Namespace != null)
                .Where(x => !x.Namespace.StartsWith("Umbraco.Core")))
            {
                //This is a special case because we have legacy handlers that are not of type IApplicationEventHandler and only 
                // of type IUmbracoStartupHandler which will throw if we try to remove them here because those are handled on
                // an internal object inside of ApplicationEventsResolver. 
                if (typeof(IApplicationEventHandler).IsAssignableFrom(startupHandler))
                {
                    appEventsResolver.CallMethod("RemoveType", m => m.First(x => x.GetParameters().Length == 1), startupHandler);
                }
                else if (typeof(IApplicationStartupHandler).IsAssignableFrom(startupHandler))
                {
                    //remove all legacy handlers
                    legacyAppEventsResolver.CallMethod("RemoveType", m => m.First(x => x.GetParameters().Length == 1), startupHandler);
                }
            }
        }
                
        private void ConfigureUmbracoSettings(Configuration config)
        {
            var umbSettings = (IUmbracoSettingsSection)config.GetSection("umbracoConfiguration/settings");
            UmbracoConfig.For.SetUmbracoSettings(umbSettings);
        }

        //private void ConfigureFileSystemProviders(Configuration config)
        //{
        //    var fileSystems = (FileSystemProvidersSection)config.GetSection("umbracoConfiguration/FileSystemProviders");
        //    FileSystemProviderManager.SetCurrent(new FileSystemProviderManager(fileSystems));
        //}
    }
}
