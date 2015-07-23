using System.Collections.ObjectModel;
using System.Windows;
using PerformanceMonitor.ViewModels;

namespace PerformanceMonitor.Views
{
    /// <summary>
    /// Interaction logic for CategoriesView.xaml
    /// </summary>
    public partial class CategoriesView
    {
        public CategoriesView()
        {
            DataContextChanged += OnDataContextChanged;

            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue as CategoriesViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedCounters = Counters.SelectedItems as ObservableCollection<object>;
                viewModel.SelectedInstances = Instances.SelectedItems as ObservableCollection<object>;
            }
        }
    }
}
