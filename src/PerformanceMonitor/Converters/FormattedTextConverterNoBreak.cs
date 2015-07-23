using System;
using System.Globalization;
using System.Linq;
using System.Windows.Documents;
using Avalon.Windows.Converters;

namespace PerformanceMonitor.Converters
{
    public class FormattedTextConverterNoBreak : FormattedTextConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = base.Convert(value, targetType, parameter, culture);
            var list = result as Inline[];
            if (list?.LastOrDefault() is LineBreak)
            {
                var newList = new Inline[list.Length - 1];
                Array.Copy(list, newList, list.Length - 1);
                return newList;
            }
            return result;
        }
    }
}
