using System;
using System.Globalization;
using System.Windows;
using Avalon.Windows.Converters;

namespace PerformanceMonitor.Converters
{
    public class BooleanToVisibilityConverter : ValueConverter
    {
        public BooleanToVisibilityConverter()
        {
            TrueVisibility = Visibility.Visible;
            FalseVisibility = Visibility.Collapsed;
        }

        public Visibility TrueVisibility { get; set; }
        
        public Visibility FalseVisibility { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as bool? == true ? TrueVisibility : FalseVisibility;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Visibility? == TrueVisibility;
        }
    }
}
