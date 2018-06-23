//using BenchmarkDotNet.Engines;
//using System;

//namespace UmbracoBenchmarks.Tools.Tests
//{
//    //TODO: This should work but it doesn't, the global setup method on the benchmark is always called

//    /// <summary>
//    /// Custom engine factory to specify a global actions without having to specify these on the benchmarks
//    /// </summary>
//    public class UmbracoEngineFactory : EngineFactory, IEngineFactory
//    {
//        public UmbracoEngineFactory(Action globalSetupAction, Action globalCleanupAction)
//        {
//            GlobalSetupAction = globalSetupAction;
//            GlobalCleanupAction = globalCleanupAction;
//        }

//        public Action GlobalSetupAction { get; }
//        public Action GlobalCleanupAction { get; }

//        IEngine IEngineFactory.Create(EngineParameters engineParameters)
//        {
//            engineParameters.GlobalSetupAction = () =>
//            {
//                Console.WriteLine("-------------------------GlobalSetupAction");
//                GlobalSetupAction();
//                engineParameters.GlobalSetupAction?.Invoke();
//            };
//            engineParameters.GlobalCleanupAction = () =>
//            {
//                Console.WriteLine("-------------------------GlobalCleanupAction");
//                GlobalCleanupAction();
//                engineParameters.GlobalCleanupAction?.Invoke();
//            };

//            return Create(engineParameters);
//        }
//    }


//}
