using System;
using System.Configuration.Provider;
using System.Reflection;

namespace UmbracoBenchmarks.Tools
{
    /// <summary>
    /// Used to hack the membership provider in there
    /// </summary>
    public static class ProviderUtil
    {
        static private FieldInfo providerCollectionReadOnlyField;

        static ProviderUtil()
        {
            Type t = typeof(ProviderCollection);
            providerCollectionReadOnlyField = t.GetField("_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        static public void AddTo(this ProviderBase provider, ProviderCollection pc)
        {
            bool prevValue = (bool)providerCollectionReadOnlyField.GetValue(pc);
            if (prevValue)
                providerCollectionReadOnlyField.SetValue(pc, false);

            pc.Add(provider);

            if (prevValue)
                providerCollectionReadOnlyField.SetValue(pc, true);
        }
    }
}
