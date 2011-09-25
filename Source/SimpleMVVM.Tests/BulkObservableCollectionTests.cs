using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using SimpleMVVM.Collections;

namespace SimpleMVVM.Tests
{
    [TestFixture]
    public class BulkObservableCollectionTests
    {
        [Test]
        public void TestAdd()
        {
            List<PropertyChangedEventArgs> propertyChangedArgs;
            List<NotifyCollectionChangedEventArgs> collectionChangedArgs;
            var collection = CreateCollection<int>(out propertyChangedArgs, out collectionChangedArgs);

            collection.Add(100);

            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[0], Is.EqualTo(100));

            Assert.That(propertyChangedArgs.Count, Is.EqualTo(2));
            Assert.That(propertyChangedArgs[0].PropertyName, Is.EqualTo("Count"));
            Assert.That(propertyChangedArgs[1].PropertyName, Is.EqualTo("Items[]"));

            Assert.That(collectionChangedArgs.Count, Is.EqualTo(1));
            Assert.That(collectionChangedArgs[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            Assert.That(collectionChangedArgs[0].NewItems.Count, Is.EqualTo(1));
            Assert.That(collectionChangedArgs[0].NewItems[0], Is.EqualTo(100));
        }

        [Test]
        public void TestAddRange()
        {
            List<PropertyChangedEventArgs> propertyChangedArgs;
            List<NotifyCollectionChangedEventArgs> collectionChangedArgs;
            var collection = CreateCollection<int>(out propertyChangedArgs, out collectionChangedArgs);

            collection.AddRange(Enumerable.Range(1, 10));

            Assert.That(collection.Count, Is.EqualTo(10));
            for (var i = 0; i < 10; i++)
            {
                Assert.That(collection[i], Is.EqualTo(i + 1));
            }

            Assert.That(propertyChangedArgs.Count, Is.EqualTo(2));
            Assert.That(propertyChangedArgs[0].PropertyName, Is.EqualTo("Count"));
            Assert.That(propertyChangedArgs[1].PropertyName, Is.EqualTo("Items[]"));

            Assert.That(collectionChangedArgs.Count, Is.EqualTo(1));
            Assert.That(collectionChangedArgs[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
        }

        [Test]
        public void TestFindWithNullPredicate()
        {
            var collection = CreateCollection(1, 2, 3, 4, 5);
            Assert.That(
                () => collection.Find(null),
                Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void TestEndBulkOperationWithotBegin()
        {
            var collection = CreateCollection<int>();
            Assert.That(
                () => collection.EndBulkOperation(),
                Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void TestBinarySearch1()
        {
            var collection = CreateCollection(1, 2, 4, 5);

            var index = collection.BinarySearch(1);

            Assert.That(index, Is.EqualTo(0));
        }

        [Test]
        public void TestBinarySearch2()
        {
            var collection = CreateCollection(1, 2, 4, 5);

            var index = collection.BinarySearch(2);

            Assert.That(index, Is.EqualTo(1));
        }

        [Test]
        public void TestBinarySearch3()
        {
            var collection = CreateCollection(1, 2, 4, 5);

            var index = collection.BinarySearch(3);

            Assert.That(index, Is.EqualTo(~2));
        }

        [Test]
        public void TestBinarySearch4()
        {
            var collection = CreateCollection(1, 2, 4, 5);

            var index = collection.BinarySearch(4);

            Assert.That(index, Is.EqualTo(2));
        }

        [Test]
        public void TestBinarySearch5()
        {
            var collection = CreateCollection(1, 2, 4, 5);

            var index = collection.BinarySearch(5);

            Assert.That(index, Is.EqualTo(3));
        }

        [Test]
        public void TestBinarySearch6()
        {
            var collection = CreateCollection(1, 2, 4, 5);

            var index = collection.BinarySearch(6);

            Assert.That(index, Is.EqualTo(~4));
        }

        private static BulkObservableCollection<T> CreateCollection<T>(
            out List<PropertyChangedEventArgs> propertyChangedArgs,
            out List<NotifyCollectionChangedEventArgs> collectionChangedArgs)
        {
            var collection = new BulkObservableCollection<T>();

            var propertyArgs = new List<PropertyChangedEventArgs>();
            collection.PropertyChanged += (s, e) => propertyArgs.Add(e);

            var collectionArgs = new List<NotifyCollectionChangedEventArgs>();
            collection.CollectionChanged += (s, e) => collectionArgs.Add(e);

            propertyChangedArgs = propertyArgs;
            collectionChangedArgs = collectionArgs;

            return collection;
        }

        private static BulkObservableCollection<T> CreateCollection<T>(IEnumerable<T> items = null)
        {
            var collection = new BulkObservableCollection<T>();

            if (items != null)
            {
                collection.AddRange(items);
            }

            return collection;
        }

        private static BulkObservableCollection<T> CreateCollection<T>(params T[] items)
        {
            var collection = new BulkObservableCollection<T>();
            collection.AddRange(items);

            return collection;
        }
    }
}
