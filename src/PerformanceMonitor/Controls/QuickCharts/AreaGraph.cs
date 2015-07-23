using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Facilitates rendering of area graphs.
    /// </summary>
    public class AreaGraph : SerialGraph
    {
        static AreaGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AreaGraph), new FrameworkPropertyMetadata(typeof(AreaGraph)));
        }

        /// <summary>
        /// Instantiates AreaGraph.
        /// </summary>
        public AreaGraph()
        {
            _areaGraph = new Polygon();

            BindBrush();
        }

        private void BindBrush()
        {
            var brushBinding = new Binding("Brush") { Source = this };
            _areaGraph.SetBinding(Shape.FillProperty, brushBinding);
        }

        private Canvas _graphCanvas;
        private readonly Polygon _areaGraph;

        /// <summary>
        /// Applies control template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _graphCanvas = (Canvas)GetTemplateChild("PART_GraphCanvas");
            _graphCanvas?.Children.Add(_areaGraph);
        }

        /// <summary>
        /// Renders area graph.
        /// </summary>
        public override void Render()
        {
            var newPoints = GetAreaPoints();
            if (_areaGraph.Points.Count != newPoints.Count)
            {
                _areaGraph.Points = newPoints;
            }
            else
            {
                for (var i = 0; i < newPoints.Count; i++)
                {
                    if (!_areaGraph.Points[i].Equals(newPoints[i]))
                    {
                        _areaGraph.Points = newPoints;
                        break;
                    }
                }
            }
        }

        private PointCollection GetAreaPoints()
        {
            var points = new PointCollection();

            if (Locations == null)
                return points;

            CopyLocationsToPoints(points);

            if (points.Count > 0)
            {
                points.Insert(0, new Point(points[0].X, GroundLevel));
                points.Add(new Point(points[points.Count - 1].X, GroundLevel));
            }
            return points;
        }

        private void CopyLocationsToPoints(PointCollection points)
        {
            // copy Points from location cause SL doesn't support PointColleciton() with parameter
            foreach (var point in Locations)
            {
                if (point != null)
                {
                    points.Add(point.Value);
                }
            }
        }
    }
}