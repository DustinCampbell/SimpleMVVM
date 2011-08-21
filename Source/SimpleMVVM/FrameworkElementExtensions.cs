using System.Windows;

namespace SimpleMVVM
{
    public static class FrameworkElementExtensions
    {
        public static T FindName<T>(this FrameworkElement frameworkElement, string name)
        {
            return (T)frameworkElement.FindName(name);
        }
    }
}
