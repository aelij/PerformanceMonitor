using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PerformanceMonitor.Controls
{
    public static class TextBoxExtensions
    {
        public static readonly DependencyProperty HandlesDeleteCommandProperty = DependencyProperty.RegisterAttached(
            "HandlesDeleteCommand", typeof(bool), typeof(TextBoxExtensions), new FrameworkPropertyMetadata(OnHandlesDeleteCommandChanged));

        private static void OnHandlesDeleteCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = ((TextBox)d);
            if ((bool)e.NewValue)
            {
                textBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, ExecuteDelete, CanExecuteDelete));
            }
            else
            {
                var bindings = textBox.CommandBindings.OfType<CommandBinding>()
                    .Where(t => t.Command == ApplicationCommands.Delete)
                    .ToArray();
                foreach (var commandBinding in bindings)
                {
                    textBox.CommandBindings.Remove(commandBinding);
                }
            }
        }

        private static void CanExecuteDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((TextBox)sender).Text.Length > 0;
        }

        private static void ExecuteDelete(object sender, ExecutedRoutedEventArgs e)
        {
            ((TextBox)sender).Clear();
        }

        public static void SetHandlesDeleteCommand(TextBox element, bool value)
        {
            element.SetValue(HandlesDeleteCommandProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetHandlesDeleteCommand(TextBox element)
        {
            return (bool)element.GetValue(HandlesDeleteCommandProperty);
        }
    }
}
