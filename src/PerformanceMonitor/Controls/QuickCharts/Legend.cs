using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Represents chart legend.
    /// </summary>
    public class Legend : ItemsControl
    {
        static Legend()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Legend), new FrameworkPropertyMetadata(typeof(Legend)));
        }

        /// <summary>
        /// Instantiates Legend.
        /// </summary>
        public Legend()
        {
            ItemsSource = _itemsSource;
        }

        private readonly ObservableCollection<LegendItem> _itemsSource = new ObservableCollection<LegendItem>();

        private IEnumerable<ILegendItem> _legendItemsSource;

        /// <summary>
        /// Gets or sets legend item source.
        /// </summary>
        public IEnumerable<ILegendItem> LegendItemsSource
        {
            get { return _legendItemsSource; }
            set
            {
                // ReSharper disable SuspiciousTypeConversion.Global
                var notifyCollectionChanged = value as INotifyCollectionChanged;
                // ReSharper restore SuspiciousTypeConversion.Global
                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged += OnLegendItemsSourceCollectionChanged;
                }
                _legendItemsSource = value;
                _itemsSource.Clear();
                AddLegendItems(value.ToList());
            }
        }

        private void OnLegendItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _itemsSource.Clear();
            }
            else
            {
                RemoveLegendItems(e.OldItems);
                AddLegendItems(e.NewItems);
            }
        }

        private void RemoveLegendItems(IList items)
        {
            if (items != null)
            {
                foreach (ILegendItem item in items)
                {
                    _itemsSource.Remove(_itemsSource.First(p => p.OriginalItem == item));
                }
            }
        }

        private void AddLegendItems(IList items)
        {
            if (items != null)
            {
                foreach (ILegendItem item in items)
                {
                    _itemsSource.Add(new LegendItem(item));
                }
            }
        }
    }
}
