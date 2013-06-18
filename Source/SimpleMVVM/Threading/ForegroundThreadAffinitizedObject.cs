using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SimpleMVVM.Threading
{
    public class ForegroundThreadAffinitizedObject
    {
        private static readonly Thread foregroundThread;
        protected static readonly TaskScheduler ForegroundTaskScheduler;

        static ForegroundThreadAffinitizedObject()
        {
            foregroundThread = Thread.CurrentThread;

            // Ensure that our idea of "foreground" is the same as WPF's
            if (Application.Current != null &&
                Application.Current.Dispatcher.Thread != foregroundThread)
            {
                throw new InvalidOperationException("ForegroundThreadAffintizedObject not initialized on WPF foreground thread!");
            }

            var previousContext = SynchronizationContext.Current;
            try
            {
                // Any work posted to the ForegroundTaskScheduler shouldn't block pending user input.
                // So, we use Background priority, which is one below user input.
                var newContext = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher, DispatcherPriority.Background);
                SynchronizationContext.SetSynchronizationContext(newContext);

                ForegroundTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        public ForegroundThreadAffinitizedObject()
        {
            AssertIsForeground();
        }

        public bool IsForeground()
        {
            return Thread.CurrentThread == ForegroundThreadAffinitizedObject.foregroundThread;
        }

        public void AssertIsForeground()
        {
            if (!IsForeground())
            {
                throw new InvalidOperationException("Expected to be on foreground!");
            }
        }

        public void AssertIsBackground()
        {
            if (IsForeground())
            {
                throw new InvalidOperationException("Expected to be on background!");
            }
        }

        private static bool IsInputPending()
        {
            uint result = NativeMethods.GetQueueStatus(NativeMethods.QS_INPUT);

            const uint InputMask = NativeMethods.QS_INPUT | (NativeMethods.QS_INPUT << 16);
            return (result & InputMask) != 0;
        }

        public Task InvokeBelowInputPriority(
            Action action,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsForeground() && !IsInputPending())
            {
                // Optimize to inline the action if we're already on the foreground thread
                // and there's no pending user input.
                action();

                return SpecializedTasks.EmptyTask;
            }

            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, ForegroundTaskScheduler);
        }

        public Task<T> InvokeBelowInputPriority<T>(
            Func<T> function,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsForeground() && !IsInputPending())
            {
                // Optimize to inline the function if we're already on the foreground thread
                // and there's no pending user input.
                var result = function();

                return Task.FromResult(result);
            }

            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, ForegroundTaskScheduler);
        }
    }
}
