using System.Windows;
using System.Windows.Media;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Facilitates rendering of line graphs.
    /// </summary>
    public class LineGraph : SerialGraph
    {
        private Pen _pen;

        static LineGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineGraph), new FrameworkPropertyMetadata(typeof(LineGraph)));
        }

        /// <summary>
        /// Renders line graph.
        /// </summary>
        public override void Render()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Locations != null && _pen != null)
            {
                Point? previous = null;
                foreach (var location in Locations)
                {
                    if (location != null && previous != null)
                    {
                        drawingContext.DrawLine(_pen, previous.Value, location.Value);
                    }
                    previous = location;
                }
            }
        }

        protected override void OnBrushChanged(Brush brush)
        {
            base.OnBrushChanged(brush);

            InvalidatePen();
        }

        private void InvalidatePen()
        {
            _pen = new Pen(Brush, StrokeThickness);
            _pen.Freeze();
        }

        /// <summary>
        /// Identifies <see cref="StrokeThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(LineGraph),
            new FrameworkPropertyMetadata(2.0, (d, e) => ((LineGraph)d).OnStrokeThicknessChanged((double)e.NewValue)));

        protected virtual void OnStrokeThicknessChanged(double thickness)
        {
            InvalidatePen();
        }

        /// <summary>
        /// Gets or sets stroke thickness for a line graph line.
        /// This is a dependency property.
        /// The default is 2.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
    }
}
