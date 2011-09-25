﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SimpleMVVM.Collections
{
    public partial class BulkObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private int bulkOperationCount;
        private bool collectionChangedDuringBulkOperation;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, PropertyChangedEventArgsCache.GetOrCreate(name));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, T oldItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, oldItem, index));
        }

        private void OnCollectionReset()
        {
            OnCountChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCountChanged()
        {
            OnPropertyChanged("Count");
            OnItemsChanged();
        }

        private void OnItemsChanged()
        {
            OnPropertyChanged("Items[]");
        }

        protected override void ClearItems()
        {
            var hadItems = Count != 0;

            base.ClearItems();

            if (hadItems)
            {
                if (bulkOperationCount == 0)
                {
                    OnCollectionReset();
                }
                else
                {
                    collectionChangedDuringBulkOperation = true;
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (bulkOperationCount == 0)
            {
                OnCountChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
            else
            {
                collectionChangedDuringBulkOperation = true;
            }
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);

            if (bulkOperationCount == 0)
            {
                OnCountChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            }
            else
            {
                collectionChangedDuringBulkOperation = true;
            }
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);

            if (bulkOperationCount == 0)
            {
                OnItemsChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, item, oldItem, index);
            }
            else
            {
                collectionChangedDuringBulkOperation = true;
            }
        }

        public void BeginBulkOperation()
        {
            bulkOperationCount++;
        }

        public void EndBulkOperation()
        {
            if (bulkOperationCount == 0)
            {
                throw new InvalidOperationException("EndBulkOperation() called without matching call to BeginBulkOperation()");
            }

            bulkOperationCount--;

            if (bulkOperationCount == 0 && collectionChangedDuringBulkOperation)
            {
                OnCollectionReset();
                collectionChangedDuringBulkOperation = false;
            }
        }

        public T Find(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (var item in this)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            throw new InvalidOperationException("No item found.");
        }

        public int BinarySearch(int index, int length, T value, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            var low = index;
            var high = (index + length) - 1;

            while (low <= high)
            {
                var mid = low + ((high - low) / 2);
                var comp = comparer.Compare(this[mid], value);

                if (comp == 0)
                {
                    return mid;
                }

                if (comp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return ~low;
        }

        public int BinarySearch(int index, int length, T value, Comparison<T> comparison)
        {
            return comparison == null
                ? BinarySearch(index, length, value, Comparer<T>.Default)
                : BinarySearch(index, length, value, new ComparisonComparer(comparison));
        }

        public int BinarySearch(int index, int length, T value, Func<T, T, int> comparison)
        {
            return comparison == null
                ? BinarySearch(index, length, value, Comparer<T>.Default)
                : BinarySearch(index, length, value, new FuncComparer(comparison));
        }

        public int BinarySearch(int index, int length, Func<T, int> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }

            var low = index;
            var high = (index + length) - 1;

            while (low <= high)
            {
                var mid = low + ((high - low) / 2);
                var comp = comparison(this[mid]);

                if (comp == 0)
                {
                    return mid;
                }

                if (comp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return ~low;
        }

        public int BinarySearch(T value, IComparer<T> comparer)
        {
            return BinarySearch(0, Count, value, comparer);
        }

        public int BinarySearch(T value, Comparison<T> comparison)
        {
            return comparison == null
                ? BinarySearch(0, Count, value, Comparer<T>.Default)
                : BinarySearch(0, Count, value, new ComparisonComparer(comparison));
        }

        public int BinarySearch(T value, Func<T, T, int> comparison)
        {
            return comparison == null
                ? BinarySearch(0, Count, value, Comparer<T>.Default)
                : BinarySearch(0, Count, value, new FuncComparer(comparison));
        }

        public int BinarySearch(Func<T, int> comparison)
        {
            return BinarySearch(0, Count, comparison);
        }

        public int BinarySearch(T value)
        {
            return BinarySearch(0, Count, value, Comparer<T>.Default);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            BeginBulkOperation();
            try
            {
                var list = items as IList<T>;
                if (list != null)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        Add(list[i]);
                    }
                }
                else
                {
                    {
                        foreach (var item in items)
                        {
                            Add(item);
                        }
                    }
                }
            }
            finally
            {
                EndBulkOperation();
            }
        }

        public ReadOnlyBulkObservableCollection<T> AsReadOnly()
        {
            return new ReadOnlyBulkObservableCollection<T>(this);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
