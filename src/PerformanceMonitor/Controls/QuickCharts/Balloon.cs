using System.Windows;
using System.Windows.Controls;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Represents a value balloon (tooltip).
    /// </summary>
    public class Balloon : Control
    {
        static Balloon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Balloon), new FrameworkPropertyMetadata(typeof(Balloon)));
        }

        /// <summary>
        /// Instantiates Balloon.
        /// </summary>
        public Balloon()
        {
            SetCurrentValue(IsHitTestVisibleProperty, false);
        }

        /// <summary>
        /// Identifies <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (Balloon),
            new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets balloon text.
        /// This is a dependency property.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
