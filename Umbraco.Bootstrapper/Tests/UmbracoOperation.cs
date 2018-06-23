using BenchmarkDotNet.Attributes;

namespace UmbracoBenchmarks.Tools.Tests
{
    public abstract class UmbracoOperation
    {
        [GlobalSetup]
        public virtual void Setup()
        {
            GlobalSetupCallbacks.RunSetupActions();
        }

        [GlobalCleanup]
        public virtual void Cleanup()
        {
            GlobalSetupCallbacks.RunCleanupActions();
        }
    }


}
