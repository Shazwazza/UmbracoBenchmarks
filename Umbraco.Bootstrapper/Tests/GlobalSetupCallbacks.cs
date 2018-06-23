using System;
using System.Collections.Generic;

namespace UmbracoBenchmarks.Tools.Tests
{
    public static class GlobalSetupCallbacks
    {
        private static readonly List<Action> _setups = new List<Action>();
        private static readonly List<Action> _cleanups = new List<Action>();
        
        public static void AddSetup(Action action)
        {
            _setups.Add(action);
        }
        public static void RunSetupActions()
        {
            foreach (var s in _setups) s();
        }
        public static void AddCleanup(Action action)
        {
            _cleanups.Add(action);
        }
        public static void RunCleanupActions()
        {
            foreach (var s in _cleanups) s();
        }
    }


}
