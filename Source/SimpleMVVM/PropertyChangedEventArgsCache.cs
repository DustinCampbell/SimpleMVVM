using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleMVVM
{
    internal static class PropertyChangedEventArgsCache
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> eventArgsCache =
            new Dictionary<string, PropertyChangedEventArgs>();

        private static readonly object gate = new object();

        public static PropertyChangedEventArgs GetOrCreate(string name)
        {
            PropertyChangedEventArgs eventArgs;

            lock (gate)
            {
                if (!eventArgsCache.TryGetValue(name, out eventArgs))
                {
                    eventArgs = new PropertyChangedEventArgs(name);
                    eventArgsCache.Add(name, eventArgs);
                }
            }

            return eventArgs;
        }
    }
}
