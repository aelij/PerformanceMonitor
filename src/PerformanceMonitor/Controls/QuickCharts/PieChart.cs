using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PerformanceMonitor.Utilities;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Displays pie charts.
    /// </summary>
    public class PieChart : Control
    {
        static PieChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(typeof(PieChart)));
        }

        /// <summary>
        /// Instantiates PieChart.
        /// </summary>
        public PieChart()
        {
            _slices.CollectionChanged += OnSlicesCollectionChanged;

            Padding = new Thickness(10);
        }


        private Balloon _balloon;

        //// LEGEND

        private Legend _legend;

        /// <summary>
        /// Identifies <see cref="LegendVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendVisibilityProperty = DependencyProperty.Register(
            "LegendVisibility", typeof(Visibility), typeof(PieChart),
            new PropertyMetadata(Visibility.Visible)
            );

        /// <summary>
        /// Gets or sets visibility of the chart legend.
        /// This is a dependency property.
        /// The default is Visible.
        /// </summary>
        public Visibility LegendVisibility
        {
            get { return (Visibility)GetValue(LegendVisibilityProperty); }
            set { SetValue(LegendVisibilityProperty, value); }
        }


        /// <summary>
        /// Identifies <see cref="DataSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            "DataSource", typeof(IEnumerable), typeof(PieChart),
            new PropertyMetadata(null, OnDataSourcePropertyChanged));

        /// <summary>
        /// Gets or sets data source for the chart.
        /// This is a dependency property.
        /// </summary>
        public IEnumerable DataSource
        {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        private static void OnDataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (PieChart)d;
            DetachOldDataSourceCollectionChangedListener(chart, e.OldValue);
            AttachDataSourceCollectionChangedListener(chart, e.NewValue);
            chart.ProcessData();
        }

        private static void DetachOldDataSourceCollectionChangedListener(PieChart chart, object dataSource)
        {
            var notifyCollectionChanged = dataSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged -= chart.OnDataSourceCollectionChanged;
            }
        }

        private static void AttachDataSourceCollectionChangedListener(PieChart chart, object dataSource)
        {
            var notifyCollectionChanged = dataSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += chart.OnDataSourceCollectionChanged;
            }
        }

        private void OnDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: implement intelligent mechanism to hanlde multiple changes in one batch
            ProcessData();
        }

        /// <summary>
        /// Identifies <see cref="TitleMemberPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleMemberPathProperty = DependencyProperty.Register(
            "TitleMemberPath", typeof(string), typeof(PieChart),
            new PropertyMetadata(null, OnMemberPathPropertyChanged)
            );

        /// <summary>
        /// Gets or sets path to the property holding slice titles in data source.
        /// This is a dependency property.
        /// </summary>
        public string TitleMemberPath
        {
            get { return (string)GetValue(TitleMemberPathProperty); }
            set { SetValue(TitleMemberPathProperty, value); }
        }

        /// <summary>
        /// Identifies <see cref="ValueMemberPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueMemberPathProperty = DependencyProperty.Register(
            "ValueMemberPath", typeof(string), typeof(PieChart),
            new PropertyMetadata(null, OnMemberPathPropertyChanged)
            );

        /// <summary>
        /// Gets or sets path to the member in the datasource holding values for the slice.
        /// This is a dependency property.
        /// </summary>
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        private static void OnMemberPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = d as PieChart;
            chart?.ProcessData();
        }

        private readonly List<string> _titles = new List<string>();
        private readonly List<double> _values = new List<double>();
        private double _total;

        private void ProcessData()
        {
            if (DataSource != null)
            {
                SetData();
                ReallocateSlices();
            }
            else
            {
                _titles.Clear();
                _values.Clear();
                _total = 0;
            }
            //InvalidateArrange();
            RenderSlices();
        }

        private void SetData()
        {
            _titles.Clear();
            _values.Clear();
            if (!string.IsNullOrEmpty(TitleMemberPath) && !string.IsNullOrEmpty(ValueMemberPath))
            {
                var titleEval = new BindingEvaluator(TitleMemberPath);
                var valueEval = new BindingEvaluator(ValueMemberPath);
                foreach (var dataItem in DataSource)
                {
                    _titles.Add(titleEval.Eval(dataItem).ToString());
                    _values.Add((double)valueEval.Eval(dataItem));
                    if (dataItem is INotifyPropertyChanged)
                    {
                        (dataItem as INotifyPropertyChanged).PropertyChanged -= OnDataSourceItemPropertyChanged;
                        (dataItem as INotifyPropertyChanged).PropertyChanged += OnDataSourceItemPropertyChanged;
                    }
                }
                _total = _values.Sum();
            }
        }

        private void OnDataSourceItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TitleMemberPath || e.PropertyName == ValueMemberPath)
            {
                ProcessData();
            }
        }

        private void ReallocateSlices()
        {
            if (_values.Count > _slices.Count)
            {
                AddSlices();
            }
            else if (_values.Count < _slices.Count)
            {
                RemoveSlices();
            }
            SetSliceData();
        }

        private void SetSliceData()
        {
            double runningTotal = 0;
            for (var i = 0; i < _slices.Count; i++)
            {
                // title
                _slices[i].Title = _titles[i];
                SetSliceBrush(i);

                // angle
                ((RotateTransform)_slices[i].RenderTransform).Angle = (Math.Abs(_total) > double.Epsilon
                                                                            ? runningTotal / _total * 360
                                                                            : 360.0 / _slices.Count * i);
                runningTotal += _values[i];

                // tooltip
                var tooltipContent = _slices[i].Title + " : " + _values[i] + " (" +
                                        (Math.Abs(_total) > double.Epsilon ? _values[i] / _total : 1.0 / _slices.Count).ToString("0.#%") + ")";
                _slices[i].ToolTipText = tooltipContent;
            }
            UpdateLegend();
        }

        private void SetSliceBrush(int index)
        {
            var brushes = _brushes.Count > 0 ? _brushes : _presetBrushes;
            var brushCount = brushes.Count;
            _slices[index].Brush = brushes[index % brushCount];
        }

        private void RemoveSlices()
        {
            for (var i = _slices.Count - 1; i >= _values.Count; i--)
            {
                RemoveSliceFromCanvas(_slices[i]);
                _slices.RemoveAt(i);
            }
        }

        private void RemoveSliceFromCanvas(Slice slice)
        {
            if (_sliceCanvas != null && _sliceCanvas.Children.Contains(slice))
            {
                _sliceCanvas.Children.Remove(slice);
            }
        }

        private void AddSlices()
        {
            for (var i = _slices.Count; i < _values.Count; i++)
            {
                var slice = new Slice { RenderTransform = new RotateTransform(), RenderTransformOrigin = new Point(0, 0) };
                _slices.Add(slice);
                slice.MouseEnter += OnSliceMouseEnter;
                slice.MouseLeave += OnSliceMouseLeave;
                slice.MouseMove += OnSliceMouseMove;
                AddSliceToCanvas(slice);
            }
        }

        private void OnSliceMouseEnter(object sender, MouseEventArgs e)
        {
            DisplayBalloon(sender as Slice, e.GetPosition(_sliceCanvasDecorator));
        }

        private void OnSliceMouseMove(object sender, MouseEventArgs e)
        {
            DisplayBalloon(sender as Slice, e.GetPosition(_sliceCanvasDecorator));
        }

        private void OnSliceMouseLeave(object sender, MouseEventArgs e)
        {
            HideBaloon();
        }

        private void AddSlicesToCanvas()
        {
            foreach (var slice in _slices)
            {
                AddSliceToCanvas(slice);
            }
        }

        private void AddSliceToCanvas(Slice slice)
        {
            if (_sliceCanvas != null && !_sliceCanvas.Children.Contains(slice))
            {
                _sliceCanvas.Children.Add(slice);
            }
        }

        private Canvas _sliceCanvas;
        private Border _sliceCanvasDecorator;

        /// <summary>
        /// Applies control template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _sliceCanvasDecorator = (Border)GetTemplateChild("PART_SliceCanvasDecorator");
            if (_sliceCanvasDecorator != null)
            {
                _sliceCanvasDecorator.SizeChanged += OnGraphCanvasDecoratorSizeChanged;
            }
            _sliceCanvas = (Canvas)GetTemplateChild("PART_SliceCanvas");

            _balloon = (Balloon)GetTemplateChild("PART_Balloon");

            AddSlicesToCanvas();

            _legend = (Legend)GetTemplateChild("PART_Legend");

            UpdateLegend();
        }

        private void UpdateLegend()
        {
            if (_legend != null)
            {
                _legend.LegendItemsSource = _slices;
            }
        }

        private void OnSlicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateLegend();
        }

        private void OnGraphCanvasDecoratorSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderSlices();
        }

        private readonly ObservableCollection<Slice> _slices = new ObservableCollection<Slice>();

        private void RenderSlices()
        {
            if (_values.Count != _slices.Count)
                ReallocateSlices();

            ArrangeSlices();
            HideBaloon();
        }

        private void ArrangeSlices()
        {
            if (_sliceCanvasDecorator != null)
            {
                var center = new Point(_sliceCanvasDecorator.ActualWidth / 2, _sliceCanvasDecorator.ActualHeight / 2);
                var radius = Math.Min(_sliceCanvasDecorator.ActualWidth, _sliceCanvasDecorator.ActualHeight) / 2;
                for (var i = 0; i < _slices.Count; i++)
                {
                    _slices[i].SetDimensions(radius, (Math.Abs(_total) > double.Epsilon ? _values[i] / _total : 1.0 / _slices.Count));
                    _slices[i].SetValue(Canvas.LeftProperty, center.X);
                    _slices[i].SetValue(Canvas.TopProperty, center.Y);
                }
            }
        }

        private readonly List<Brush> _presetBrushes = new List<Brush>
            {
                new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x0F, 0x00)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x66, 0x00)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x9E, 0x01)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xFC, 0xD2, 0x02)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xF8, 0xFF, 0x01)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xB0, 0xDE, 0x09)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x04, 0xD2, 0x15)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x0D, 0x8E, 0xCF)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x0D, 0x52, 0xD1)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x0C, 0xD0)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x8A, 0x0C, 0xCF)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0x0D, 0x74)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x75, 0x4D, 0xEB)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x99, 0x99)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x00, 0x00))
            };

        private readonly List<Brush> _brushes = new List<Brush>();

        /// <summary>
        /// Gets a collection of preset brushes used for slices.
        /// </summary>
        public List<Brush> Brushes
        {
            get { return _brushes; }
            set { throw new NotSupportedException(); }
        }

        private void DisplayBalloon(Slice slice, Point position)
        {
            _balloon.Text = slice.ToolTipText;
            _balloon.Visibility = Visibility.Visible;
            _balloon.Measure(new Size(_sliceCanvasDecorator.ActualWidth, _sliceCanvasDecorator.ActualHeight));
            var balloonLeft = position.X - _balloon.DesiredSize.Width / 2;
            if (balloonLeft < 0)
            {
                balloonLeft = position.X;
            }
            else if (balloonLeft + _balloon.DesiredSize.Width > _sliceCanvasDecorator.ActualWidth)
            {
                balloonLeft = position.X - _balloon.DesiredSize.Width;
            }
            var balloonTop = position.Y - _balloon.DesiredSize.Height - 5;
            if (balloonTop < 0)
            {
                balloonTop = position.Y + 17;
            }
            _balloon.SetValue(Canvas.LeftProperty, balloonLeft);
            _balloon.SetValue(Canvas.TopProperty, balloonTop);
        }

        private void HideBaloon()
        {
            if (_balloon != null)
            {
                _balloon.Visibility = Visibility.Collapsed;
            }
        }
    }
}