using System;
using System.Globalization;
using Avalon.Windows.Converters;

namespace PerformanceMonitor.Converters
{
    public class MultiplierDoubleConverter : ValueConverter
    {
        public double Multiplier { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as double?).GetValueOrDefault() * Multiplier;
        }
    }
}
