using System.Collections.Generic;
using PerformanceMonitor.Annotations;

namespace PerformanceMonitor.ViewModels
{
    public struct ChartValue
    {
        public ChartValue(float value, float scale)
        {
            Value = value;
            Scale = scale;
        }

        public float Value { get; }

        public float Scale { get; }

        public ChartValue Rescale(float newScale)
        {
            return new ChartValue(Value / Scale * newScale, newScale);
        }
    }

    public class ChartItemViewModel
    {
        private readonly Dictionary<int, ChartValue> _values;

        public ChartItemViewModel()
        {
            Time = string.Empty;
        }

        public ChartItemViewModel(string time, Dictionary<int, ChartValue> values)
        {
            Time = time;
            _values = values;
        }

        [UsedImplicitly]
        public string Time { get; set; }

        [UsedImplicitly]
        public float? this[int id]
        {
            get
            {
                if (id < 0) return 0;
                ChartValue result;
                if (_values != null && _values.TryGetValue(id, out result))
                {
                    return result.Value;
                }
                return null;
            }
        }

        public bool TryRescale(int id, float newScale)
        {
            ChartValue value;
            if (_values != null && _values.TryGetValue(id, out value))
            {
                _values[id] = value.Rescale(newScale);
                return true;
            }
            return false;
        }
    }
}
