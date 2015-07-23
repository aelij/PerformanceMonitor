using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Base class for graphs in serial chart.
    /// </summary>
    public abstract class SerialGraph : Control, ILegendItem
    {
        /// <summary>
        /// Event is raised when ValueMemberPath changes.
        /// </summary>
        public event EventHandler<DataPathEventArgs> ValueMemberPathChanged;

        /// <summary>
        /// Identifies <see cref="ValueMemberPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueMemberPathProperty = DependencyProperty.Register(
            "ValueMemberPath", typeof(string), typeof(SerialGraph),
            new PropertyMetadata(null, OnValueMemberPathPropertyChanged)
            );

        /// <summary>
        /// Gets or sets path to the member in the datasource holding values for this graph.
        /// This is a dependency property.
        /// </summary>
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        private static void OnValueMemberPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graph = (SerialGraph)d;
            graph.ValueMemberPathChanged?.Invoke(graph, new DataPathEventArgs(e.NewValue as string));
        }

        /// <summary>
        /// Gets locations (coordinates) of data points for the graph.
        /// </summary>
        protected IList<Point?> Locations { get; private set; }

        /// <summary>
        /// Gets Y-coordinate of 0 or a value closest to 0.
        /// </summary>
        protected double GroundLevel { get; private set; }

        /// <summary>
        /// Sets point coordinates and ground level.
        /// </summary>
        /// <param name="locations">Locations (coordinates) of data points for the graph.</param>
        /// <param name="groundLevel">Y-coordinate of 0 value or value closest to 0.</param>
        public void SetPointLocations(IList<Point?> locations, double groundLevel)
        {
            Locations = locations;
            GroundLevel = groundLevel;
        }

        /// <summary>
        /// When implemented in inheriting classes, renders the graphs visual.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Gets single x-axis step size (the distance between 2 neighbour data points).
        /// </summary>
        protected double XStep
        {
            get
            {
                var locs = NonNullLocations;
                if (locs.Count > 1)
                {
                    return locs[1].X - locs[0].X;
                }
                if (locs.Count == 1)
                {
                    return locs[0].X * 2;
                }
                return 0;
            }
        }

        protected IList<Point> NonNullLocations
        {
            get { return Locations.Where(t => t != null).Select(t => t.GetValueOrDefault()).ToArray(); }
        }

        /// <summary>
        /// Identifies <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(SerialGraph),
            new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the title of the graph.
        /// This is a dependency property.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies <see cref="Brush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof(Brush), typeof(SerialGraph),
            new PropertyMetadata((d, e) => ((SerialGraph)d).OnBrushChanged((Brush)e.NewValue))
            );

        protected virtual void OnBrushChanged(Brush brush)
        {
        }

        /// <summary>
        /// Gets or sets brush for the graph.
        /// This is a dependency property.
        /// </summary>
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }
    }
}