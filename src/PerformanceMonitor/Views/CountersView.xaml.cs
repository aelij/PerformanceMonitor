using System.Windows.Controls;
using System.Windows.Input;
using PerformanceMonitor.Models;
using PerformanceMonitor.ViewModels;
using System.Linq;

namespace PerformanceMonitor.Views
{
    /// <summary>
    /// Interaction logic for CountersView.xaml
    /// </summary>
    public partial class CountersView
    {
        public CountersView()
        {
            InitializeComponent();
        }

        private CountersViewModel ViewModel => (CountersViewModel) DataContext;

        private void DataGridKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (var counterData in Counters.SelectedItems.Cast<CounterData>().ToArray())
                {
                    ViewModel.Remove(counterData);
                }
            }
        }

        private void Counters_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (CounterData counterData in e.AddedItems)
                {
                    ViewModel.SelectedCounters.Add(counterData);
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (CounterData counterData in e.RemovedItems)
                {
                    ViewModel.SelectedCounters.Remove(counterData);
                }
            }
        }
    }
}
