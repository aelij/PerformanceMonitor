using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using PerformanceMonitor.ViewModels;

namespace PerformanceMonitor.Models
{
    public class CounterData : ViewModel
    {
        private static int _globalId;

        public string Category => Key.Category;
        public string Name => Key.Name;
        public string Instance => Key.Instance;
        private CounterSample _previousSample;
        private CounterSample _currentSample;
        private float? _value;

        public int Id { get; private set; }

        private float? _minimum;
        private float? _maximum;
        private float _currentScale;

        public float? Value
        {
            get { return _value; }
            private set { RaiseAndSetIfChanged(ref _value, value); }
        }

        public float? ScaledValue => _value * CurrentScale;

        public CounterData(CounterKey key, Color color)
        {
            Key = key;
            Color = color;
            _previousSample = CounterSample.Empty;

            Id = Interlocked.Increment(ref _globalId);
        }

        public float? Minimum
        {
            get { return _minimum; }
            private set { RaiseAndSetIfChanged(ref _minimum, value); }
        }

        public float? Maximum
        {
            get { return _maximum; }
            private set { RaiseAndSetIfChanged(ref _maximum, value); }
        }

        public CounterSample CurrentSample
        {
            get { return _currentSample; }
            private set
            {
                _currentSample = value;
                Value = CounterSample.Calculate(_previousSample, value);
                if (Minimum == null || Minimum > Value)
                {
                    Minimum = Value;
                }
                if (Maximum == null || Maximum < Value)
                {
                    Maximum = Value;
                }
                _previousSample = _currentSample;
                _currentSample = value;
            }
        }

        public CounterKey Key { get; }

        public float CurrentScale
        {
            get { return _currentScale; }
            set
            {
                RaiseAndSetIfChanged(ref _currentScale, value);
                RaisePropertyChanged(() => ScaledValue);
            }
        }

        public Color Color { get; }

        public float? CalculateScale()
        {
            if (Maximum == null)
            {
                return null;
            }

            var max = Maximum.Value;
            if (max < 100) max = 100;

            var scale = (100 / Math.Pow(10, Math.Ceiling(Math.Log10(max))));
            return (float)scale;
        }

        public void Read(DateTime timestamp, CounterSample sample)
        {
            if (_previousSample == CounterSample.Empty)
            {
                _previousSample = sample;
            }
            else
            {
                CurrentSample = sample;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (CurrentScale == 0)
                {
                    CurrentScale = CalculateScale().GetValueOrDefault(1);
                }
            }
        }
    }
}
