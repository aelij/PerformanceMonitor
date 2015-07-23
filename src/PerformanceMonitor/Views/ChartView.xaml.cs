using System.Linq;
using System.Windows;
using System.Windows.Media;
using PerformanceMonitor.Controls.QuickCharts;
using PerformanceMonitor.Models;
using PerformanceMonitor.Utilities;
using PerformanceMonitor.ViewModels;

namespace PerformanceMonitor.Views
{
    /// <summary>
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView
    {
        public ChartView()
        {
            DataContextChanged += OnDataContextChanged;
            InitializeComponent();

            Chart.PresetBrushes = VividColors.All.Select(color => (Brush)new SolidColorBrush(color)).ToArray();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.OldValue as CountersViewModel;
            if (viewModel != null)
            {
                viewModel.CounterAdded -= ViewModelOnCounterAdded;
                viewModel.CounterRemoved -= ViewModelOnCounterRemoved;
            }
            viewModel = e.NewValue as CountersViewModel;
            if (viewModel != null)
            {
                viewModel.CounterAdded += ViewModelOnCounterAdded;
                viewModel.CounterRemoved += ViewModelOnCounterRemoved;
            }
        }

        private void ViewModelOnCounterRemoved(CounterData counterData)
        {
            var graph = Chart.Graphs.FirstOrDefault(t => Equals(t.Tag, counterData.Key));
            if (graph != null)
            {
                Chart.Graphs.Remove(graph);
            }
        }

        private void ViewModelOnCounterAdded(CounterData counterData)
        {
            var graph = new LineGraph
            {
                Title = counterData.Name,
                Tag = counterData.Key,
                Brush = new SolidColorBrush(counterData.Color),
                ValueMemberPath = $"[{counterData.Id}]",
                StrokeThickness = 1,
            };
            RenderOptions.SetEdgeMode(graph, EdgeMode.Aliased);
            Chart.Graphs.Add(graph);
        }

        private void Chart_OnEvaluatingCategory(object sender, ValueEventArgs<string> e)
        {
            e.Value = ((ChartItemViewModel)e.Item).Time;
        }

        private void Chart_OnEvaluatingValueMember(object sender, ValueEventArgs<double?> e)
        {
            e.Value = ((ChartItemViewModel)e.Item)[int.Parse(e.Path.Trim('[', ']'))];
        }
    }
}
