using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using PerformanceMonitor.Models;
using PerformanceMonitor.Utilities;
using Timer = System.Timers.Timer;

namespace PerformanceMonitor.ViewModels
{
    public class CountersViewModel : ViewModel
    {
        private static readonly ChartItemViewModel _emptyItem = new ChartItemViewModel();

        private readonly CategoriesViewModel _categoriesViewModel;
        private readonly KeyedCollectionProxy<CounterKey, CounterData> _counters;
        private readonly ObservableCollection<ChartItemViewModel> _chartItems;
        private readonly Dictionary<string, List<CounterData>> _categories;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Timer _timer;
        private readonly SynchronizationContext _syncContext;

        private int _currentChartIndex;
        private int _lastColorIndex;

        private const int MaxChartItems = 100;

        public CountersViewModel(CategoriesViewModel categoriesViewModel)
        {
            _categoriesViewModel = categoriesViewModel;
            _counters = new KeyedCollectionProxy<CounterKey, CounterData>(c => c.Key);
            _chartItems = new ObservableCollection<ChartItemViewModel>(
                Enumerable.Range(0, MaxChartItems).Select(t => _emptyItem));
            
            _counters.CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            ScaleSelectedCommand = CreateCommand(ScaleSelected, () => SelectedCounters.Count > 0);

            SelectedCounters = new ObservableCollection<CounterData>();
            SelectedCounters.CollectionChanged += (sender, args) => ScaleSelectedCommand.RaiseCanExecuteChanged();
            
            _categories = new Dictionary<string, List<CounterData>>();

            _syncContext = SynchronizationContext.Current;

            categoriesViewModel.Added += Add;

            _timer = new Timer(1000);
            _timer.Elapsed += (sender, args) => Tick();
            _timer.AutoReset = false;
            _timer.Start();   
        }

        private void ScaleSelected()
        {
            foreach (var counter in SelectedCounters)
            {
                var scale = counter.CalculateScale();
                if (scale != null)
                {
                    counter.CurrentScale = scale.Value;

                    foreach (var chartItem in _chartItems)
                    {
                        chartItem.TryRescale(counter.Id, scale.Value);
                    }
                }
            }

            RaisePropertyChanged(() => ChartItems);
        }

        public void Add(CounterKey key)
        {
            CounterData item;
            if (!_counters.TryGetItem(key, out item))
            {
                if (++_lastColorIndex >= VividColors.All.Count)
                {
                    _lastColorIndex = 0;
                }
                var counterData = new CounterData(key, VividColors.All[_lastColorIndex]);
                _counters.Add(counterData);
                List<CounterData> category;
                if (!_categories.TryGetValue(key.Category, out category))
                {
                    category = new List<CounterData>();
                    _categories.Add(key.Category, category);
                }
                category.Add(counterData);
                OnCounterAdded(counterData);
            }
        }

        public event Action<CounterData> CounterAdded;

        protected virtual void OnCounterAdded(CounterData counterData)
        {
            var handler = CounterAdded;
            handler?.Invoke(counterData);
        }

        public void Remove(CounterData counterData)
        {
            _counters.Remove(counterData);
            var counterDatas = _categories[counterData.Category];
            counterDatas.Remove(counterData);
            if (counterDatas.Count == 0)
            {
                _categories.Remove(counterData.Category);
            }
            OnCounterRemoved(counterData);
            if (_categories.Count == 0)
            {
                CurrentChartIndex = 0;
                for (var i = 0; i < _chartItems.Count; ++i)
                {
                    _chartItems[i] = _emptyItem;
                }
            }
        }

        public event Action<CounterData> CounterRemoved;

        protected virtual void OnCounterRemoved(CounterData counterData)
        {
            var handler = CounterRemoved;
            handler?.Invoke(counterData);
        }

        private void Tick()
        {
            if (_categories.Count > 0)
            {
                var timestamp = DateTime.Now;
                foreach (var categoryItem in _categories)
                {
                    var category = _categoriesViewModel.GetCategory(categoryItem.Key);
                    category.Read(timestamp, categoryItem.Value);
                }

                var chartItem = new ChartItemViewModel(timestamp.ToLongTimeString(),
                                              _counters.Where(t => t.Value != null).ToArray().ToDictionary(t => t.Id, t => new ChartValue(t.ScaledValue.GetValueOrDefault(), t.CurrentScale)));

                _syncContext.Post(o =>
                {
                    _chartItems[CurrentChartIndex] = chartItem;
                    if (CurrentChartIndex + 1 < _chartItems.Count)
                    {
                        _chartItems[CurrentChartIndex + 1] = _emptyItem;
                    }

                    if (++CurrentChartIndex >= MaxChartItems)
                    {
                        CurrentChartIndex = 0;
                    }
                }, null);
            }

            _timer.Start();
        }

        public ICollectionView CountersView => _counters.CollectionView;

        public IEnumerable<ChartItemViewModel> ChartItems => _chartItems;

        public int CurrentChartIndex
        {
            get { return _currentChartIndex; }
            set { RaiseAndSetIfChanged(ref _currentChartIndex, value); }
        }

        public ObservableCollection<CounterData> SelectedCounters { get; }

        public ICommandEx ScaleSelectedCommand { get; }
    }
}
