using System.Windows;
using System.Windows.Data;

namespace PerformanceMonitor.Utilities
{
    /// <summary>
    /// Utility class to facilitate temporary binding evaluation
    /// </summary>
    public class BindingEvaluator : DependencyObject
    {
        /// <summary>
        /// Created binding evaluator and set path to the property which's value should be evaluated.
        /// </summary>
        /// <param name="propertyPath">Path to the property</param>
        public BindingEvaluator(string propertyPath)
        {
            _propertyPath = propertyPath;
        }

        private readonly string _propertyPath;

        /// <summary>
        /// Dependency property used to evaluate values.
        /// </summary>
        public static readonly DependencyProperty EvaluatorProperty = DependencyProperty.Register(
            "Evaluator", typeof (object), typeof (BindingEvaluator), null);

        /// <summary>
        /// Returns evaluated value of property on provided object source.
        /// </summary>
        /// <param name="source">Object for which property value should be evaluated</param>
        /// <returns>Value of the property</returns>
        public object Eval(object source)
        {
            ClearValue(EvaluatorProperty);
            var binding = new Binding(_propertyPath) {Mode = BindingMode.OneTime, Source = source};
            BindingOperations.SetBinding(this, EvaluatorProperty, binding);
            return GetValue(EvaluatorProperty);
        }
    }
}
