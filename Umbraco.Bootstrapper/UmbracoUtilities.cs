using System;

namespace UmbracoBenchmarks.Tools
{
    /// <summary>
    /// Shared reflection utilities for Umbraco
    /// </summary>
    /// <remarks>
    /// Used so that there is less code duplication between the version specific boot strappers
    /// </remarks>
    public static class UmbracoUtilities
    {
        public static void SetIOHelperRoot(Type ioHelperType, string umbracoFolder)
        {
            ioHelperType.CallStaticMethod("SetRootDirectory", umbracoFolder);
        }
    }
}
