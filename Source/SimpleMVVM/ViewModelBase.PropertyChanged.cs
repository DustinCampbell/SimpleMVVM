using System.ComponentModel;

namespace SimpleMVVM
{
    public abstract partial class ViewModelBase : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler propertyChangedHandler;

        protected void PropertyChanged(string name)
        {
            var handler = propertyChangedHandler;
            if (handler != null)
            {
                handler(this, PropertyChangedEventArgsCache.GetOrCreate(name));
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
