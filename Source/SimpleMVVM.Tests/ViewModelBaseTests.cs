using System;
using System.ComponentModel;
using System.Windows.Controls;
using NUnit.Framework;

namespace SimpleMVVM.Tests
{
    [TestFixture]
    [RequiresSTA]
    public class ViewModelBaseTests
    {
        private class MockViewModel : ViewModelBase
        {
            public void Test_PropertyChanged(string name)
            {
                PropertyChanged(name);
            }

            public void Test_AllPropertiesChanged()
            {
                AllPropertiesChanged();
            }
        }

        [Test(Description = "INotifyPropertyChanged.PropertyChanged fires when PropertyChanged called")]
        public void TestPropertyChangedEvent1()
        {
            var mock = new MockViewModel();
            var notifyPropertyChanged = (INotifyPropertyChanged)mock;

            var eventTriggerCount = 0;
            PropertyChangedEventArgs eventArgs = null;
            notifyPropertyChanged.PropertyChanged += (_, e) =>
            {
                eventTriggerCount++;
                eventArgs = e;
            };

            mock.Test_PropertyChanged("Test");

            Assert.That(eventTriggerCount, Is.EqualTo(1));
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.PropertyName, Is.EqualTo("Test"));
        }

        [Test(Description = "INotifyPropertyChanged.PropertyChanged fires when AllPropertiesChanged called")]
        public void TestPropertyChangedEvent2()
        {
            var mock = new MockViewModel();
            var notifyPropertyChanged = (INotifyPropertyChanged)mock;

            var eventTriggerCount = 0;
            PropertyChangedEventArgs eventArgs = null;
            notifyPropertyChanged.PropertyChanged += (_, e) =>
            {
                eventTriggerCount++;
                eventArgs = e;
            };

            mock.Test_AllPropertiesChanged();

            Assert.That(eventTriggerCount, Is.EqualTo(1));
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.PropertyName, Is.EqualTo(string.Empty));
        }

        private class MockCreateViewTestViewModel : ViewModelBase<UserControl>
        {
            public MockCreateViewTestViewModel(string viewName)
                : base(viewName)
            {
            }

            public MockCreateViewTestViewModel(Uri viewUri)
                : base(viewUri)
            {
            }

            protected override void OnViewCreated(UserControl view)
            {
                OnViewCreatedCallCount++;
                Assert.That(view, Is.SameAs(View));
            }

            public int OnViewCreatedCallCount { get; private set; }
        }

        [Test(Description = "Ensure CreateView works as expected when ViewModel is constructed from Uri")]
        public void TestCreateView1()
        {
            var mockViewUri = new Uri("/SimpleMVVM.Tests;component/MockCreateViewTestView.xaml", UriKind.Relative);
            var mock = new MockCreateViewTestViewModel(mockViewUri);

            var view = mock.CreateView();

            Assert.That(view, Is.Not.Null);
            Assert.That(mock.OnViewCreatedCallCount, Is.EqualTo(1));
        }

        [Test(Description = "Ensure CreateView works as expected when ViewModel is constructed from string")]
        public void TestCreateView2()
        {
            var mock = new MockCreateViewTestViewModel("MockCreateViewTestView");

            var view = mock.CreateView();

            Assert.That(view, Is.Not.Null);
            Assert.That(mock.OnViewCreatedCallCount, Is.EqualTo(1));
        }

        [Test(Description = "Ensure CreateView throws when called a second time")]
        public void TestCreateView3()
        {
            var mock = new MockCreateViewTestViewModel("MockCreateViewTestView");

            var view = mock.CreateView();

            Assert.That(view, Is.Not.Null);
            Assert.That(mock.OnViewCreatedCallCount, Is.EqualTo(1));

            Assert.Throws<InvalidOperationException>(() => mock.CreateView());
        }
    }
}
