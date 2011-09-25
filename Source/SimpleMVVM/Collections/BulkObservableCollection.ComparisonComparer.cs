using System;
using System.Collections.Generic;

namespace SimpleMVVM.Collections
{
    public partial class BulkObservableCollection<T>
    {
        private class ComparisonComparer : IComparer<T>
        {
            private readonly Comparison<T> comparison;

            public ComparisonComparer(Comparison<T> comparison)
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
