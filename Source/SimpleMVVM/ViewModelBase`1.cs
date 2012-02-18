using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleMVVM
{
    public abstract class ViewModelBase<TView> : ViewModelBase
        where TView : ContentControl
    {
        private readonly Uri viewUri;

        protected ViewModelBase(string viewName)
        {
            var assemblyName = GetType().Assembly.GetName().Name;
            var uriString = string.Format("/{0};component/{1}.xaml", assemblyName, viewName);

            viewUri = new Uri(uriString, UriKind.Relative);
        }

        protected ViewModelBase(Uri viewUri)
        {
            this.viewUri = viewUri;
        }

        protected TView View { get; private set; }

        protected virtual void OnViewCreated(TView view)
        {
            // Descendents can override;
        }

        public TView CreateView()
        {
            if (View != null)
            {
                throw new InvalidOperationException("View already created.");
            }

            View = (TView)Application.LoadComponent(viewUri);
            View.DataContext = this;

            OnViewCreated(View);

            SetRegisterCommands(View, this);

            return View;
        }
    }
}
