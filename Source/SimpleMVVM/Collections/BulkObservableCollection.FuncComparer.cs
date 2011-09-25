using System;
using System.Collections.Generic;

namespace SimpleMVVM.Collections
{
    public partial class BulkObservableCollection<T>
    {
        private class FuncComparer : IComparer<T>
        {
            private readonly Func<T, T, int> comparison;

            public FuncComparer(Func<T, T, int> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return comparison(x, y);
            }
        }
    }
}
