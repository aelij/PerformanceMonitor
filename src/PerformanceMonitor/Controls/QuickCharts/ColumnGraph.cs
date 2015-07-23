using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Facilitates rendering of column graphs.
    /// </summary>
    public class ColumnGraph : SerialGraph
    {
        static ColumnGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnGraph), new FrameworkPropertyMetadata(typeof(ColumnGraph)));
        }

        /// <summary>
        /// Creates ColumnGraph object.
        /// </summary>
        public ColumnGraph()
        {
            _columnGraph = new Path();
            _columnGraphGeometry = new PathGeometry();
            _columnGraph.Data = _columnGraphGeometry;

            BindBrush();
        }

        private void BindBrush()
        {
            var brushBinding = new Binding("Brush") { Source = this };
            _columnGraph.SetBinding(Shape.FillProperty, brushBinding);
        }

        private Canvas _graphCanvas;
        private readonly Path _columnGraph;
        private readonly PathGeometry _columnGraphGeometry;

        /// <summary>
        /// Applies control template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _graphCanvas = (Canvas)GetTemplateChild("PART_GraphCanvas");
            _graphCanvas?.Children.Add(_columnGraph);
        }

        /// <summary>
        /// Renders graph.
        /// </summary>
        public override void Render()
        {
            if (Locations != null)
            {
                var changeCount = Math.Min(Locations.Count, _columnGraphGeometry.Figures.Count);
                ChangeColumns(changeCount);
                var diff = Locations.Count - _columnGraphGeometry.Figures.Count;
                if (diff > 0)
                {
                    AddColumns(changeCount);
                }
                else if (diff < 0)
                {
                    RemoveColumns(changeCount);
                }
            }
        }

        private void AddColumns(int changeCount)
        {
            for (var i = changeCount; i < Locations.Count; i++)
            {
                var column = new PathFigure();
                _columnGraphGeometry.Figures.Add(column);
                for (var si = 0; si < 4; si++)
                {
                    column.Segments.Add(new LineSegment());
                }
                SetColumnSegments(i);
            }
        }

        private void RemoveColumns(int changeCount)
        {
            for (var i = _columnGraphGeometry.Figures.Count - 1; i >= changeCount; i--)
            {
                _columnGraphGeometry.Figures.RemoveAt(i);
            }
        }

        private void ChangeColumns(int changeCount)
        {
            for (var i = 0; i < changeCount; i++)
            {
                SetColumnSegments(i);
            }
        }

        private void SetColumnSegments(int index)
        {
            var width = XStep * ColumnWidthAllocation;
            var point = Locations[index];
            if (point != null)
            {
                var left = point.Value.X - width/2;
                var right = left + width;
                var y1 = GroundLevel;
                var y2 = point.Value.Y;

                _columnGraphGeometry.Figures[index].StartPoint = new Point(left, y1);
                ((LineSegment) _columnGraphGeometry.Figures[index].Segments[0]).Point = new Point(right, y1);
                ((LineSegment) _columnGraphGeometry.Figures[index].Segments[1]).Point = new Point(right, y2);
                ((LineSegment) _columnGraphGeometry.Figures[index].Segments[2]).Point = new Point(left, y2);
                ((LineSegment) _columnGraphGeometry.Figures[index].Segments[3]).Point = new Point(left, y1);
            }
        }

        /// <summary>
        /// Identifies <see cref="ColumnWidthAllocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthAllocationProperty = DependencyProperty.Register(
            "ColumnWidthAllocation", typeof(double), typeof(ColumnGraph),
            new PropertyMetadata(0.8)
            );

        /// <summary>
        /// Get or sets coefficient for allocation of space between 2 data points to a single column. 
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// A value between 0 and 1 is expected. 0 means no space and 1 means the column will occupy the whole space between 2 neighbouring data points.
        /// </remarks>
        public double ColumnWidthAllocation
        {
            get { return (double)GetValue(ColumnWidthAllocationProperty); }
            set { SetValue(ColumnWidthAllocationProperty, value); }
        }
    }
}