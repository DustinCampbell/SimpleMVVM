using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleMVVM
{
    public abstract partial class ViewModelBase : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> eventArgsCache =
            new Dictionary<string, PropertyChangedEventArgs>();

        private static readonly object gate = new object();

        private static PropertyChangedEventArgs GetOrCreateEventArgs(string name)
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

        private PropertyChangedEventHandler propertyChangedHandler;

        protected void PropertyChanged(string name)
        {
            var handler = propertyChangedHandler;
            if (handler != null)
            {
                handler(this, GetOrCreateEventArgs(name));
            }
        }

        protected void AllPropertiesChanged()
        {
            PropertyChanged(string.Empty);
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { propertyChangedHandler += value; }
            remove { propertyChangedHandler -= value; }
        }
    }
}
