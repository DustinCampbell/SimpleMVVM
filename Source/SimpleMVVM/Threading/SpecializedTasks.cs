using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMVVM.Threading
{
    public static class SpecializedTasks
    {
        public static readonly Task<bool> True = Task.FromResult<bool>(true);
        public static readonly Task<bool> False = Task.FromResult<bool>(false);
        public static readonly Task EmptyTask = Empty<object>.Default;

        public static Task<T> Default<T>()
        {
            return Empty<T>.Default;
        }

        private static class Empty<T>
        {
            public static readonly Task<T> Default = Task.FromResult<T>(default(T));
        }
    }
}
