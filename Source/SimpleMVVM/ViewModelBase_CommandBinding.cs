using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SimpleMVVM
{
    public abstract partial class ViewModelBase
    {
        private CommandBindingCollection commandBindings;

        private CommandBindingCollection CommandBindings
        {
            get
            {
                if (commandBindings == null)
                {
                    Interlocked.CompareExchange(ref commandBindings, new CommandBindingCollection(), null);
                }

                return commandBindings;
            }
        }

        private ICommand RegisterCommand(
            string text,
            string name,
            InputGesture[] inputGestures,
            ExecutedRoutedEventHandler executed,
            CanExecuteRoutedEventHandler canExecute)
        {
            var command = new RoutedUICommand(text, name, GetType(), new InputGestureCollection(inputGestures));
            var commandBinding = new CommandBinding(command, executed, canExecute);

            CommandManager.RegisterClassCommandBinding(GetType(), commandBinding);
            CommandBindings.Add(commandBinding);

            return command;
        }

        protected ICommand RegisterCommand(
            string text,
            string name,
            Action executed,
            Func<bool> canExecute,
            params InputGesture[] inputGestures)
        {
            return RegisterCommand(
                text, name, inputGestures,
                executed: (s, e) => executed(),
                canExecute: (s, e) => e.CanExecute = canExecute());
        }

        protected ICommand RegisterCommand<T>(
            string text,
            string name,
            Action<T> executed,
            Func<T, bool> canExecute,
            params InputGesture[] inputGestures)
        {
            Func<object, T> cast = x => x != null ? (T)x : default(T);

            return RegisterCommand(
                text, name, inputGestures,
                executed: (s, e) => executed(cast(e.Parameter)),
                canExecute: (s, e) => e.CanExecute = canExecute(cast(e.Parameter)));
        }

        public static readonly DependencyProperty RegisterCommandsProperty =
            DependencyProperty.RegisterAttached(
                name: "RegisterCommands",
                propertyType: typeof(ViewModelBase),
                ownerType: typeof(ViewModelBase),
                defaultMetadata: new PropertyMetadata(
                    defaultValue: null,
                    propertyChangedCallback: (dp, e) =>
                    {
                        var element = dp as UIElement;
                        if (element != null)
                        {
                            var viewModel = e.NewValue as ViewModelBase;
                            if (viewModel != null)
                            {
                                element.CommandBindings.AddRange(viewModel.CommandBindings);
                            }
                        }
                    }));

        public static ViewModelBase GetRegisterCommands(UIElement element)
        {
            return (ViewModelBase)element.GetValue(RegisterCommandsProperty);
        }

        public static void SetRegisterCommands(UIElement element, ViewModelBase value)
        {
            element.SetValue(RegisterCommandsProperty, value);
        }
    }
}
