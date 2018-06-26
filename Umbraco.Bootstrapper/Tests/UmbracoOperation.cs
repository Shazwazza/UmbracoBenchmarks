using BenchmarkDotNet.Attributes;
using Umbraco.Core;

namespace UmbracoBenchmarks.Tools.Tests
{
    public abstract class UmbracoOperation
    {
        protected ApplicationContext ApplicationContext { get; private set; }

        [GlobalSetup]
        public virtual void SetupDefault()
        {
            GlobalSetupCallbacks.RunSetupActions();
            ApplicationContext = ApplicationContext.Current;
        }

        [GlobalCleanup]
        public virtual void CleanupDefault()
        {
            GlobalSetupCallbacks.RunCleanupActions();
        }
    }


}
