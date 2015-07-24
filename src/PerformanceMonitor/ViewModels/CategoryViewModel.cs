using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PerformanceMonitor.Models;

namespace PerformanceMonitor.ViewModels
{
    public class CategoryViewModel : ViewModel
    {
        private readonly PerformanceCounterCategory _category;
        private ICollection<string> _instances;
        private ICollection<string> _counters;

        public CategoryViewModel(PerformanceCounterCategory category)
        {
            _category = category;

            Task.Run(() => Refresh());
        }

        public void Refresh()
        {
            try
            {
                var data = _category.ReadCategory();

                // ReSharper disable AssignNullToNotNullAttribute

                Counters = data.Values.Cast<InstanceDataCollection>()
                    .Select(t => t.CounterName)
                    .OrderBy(t => t, StringComparer.InvariantCultureIgnoreCase)
                    .ToArray();

                Instances = Counters.Count > 0
                                ? data.Values.Cast<InstanceDataCollection>().First()
                                      .Values.Cast<InstanceData>()
                                      .Select(t => t.InstanceName)
                                      .OrderBy(t => t, StringComparer.InvariantCultureIgnoreCase)
                                      .ToArray()
                                : new string[0];

                // ReSharper restore AssignNullToNotNullAttribute
            }
            catch (Exception)
            {
            }
        }

        public void Read(DateTime timestamp, IEnumerable<CounterData> items)
        {
            var data = _category.ReadCategory();

            foreach (var item in items)
            {
                item.Read(timestamp, data[item.Name][item.Instance].Sample);
            }
        }

        public ICollection<string> Instances
        {
            get { return _instances; }
            private set { RaiseAndSetIfChanged(ref _instances, value); }
        }

        public ICollection<string> Counters
        {
            get { return _counters; }
            private set { RaiseAndSetIfChanged(ref _counters, value); }
        }

        public string Name => _category.CategoryName;

        public override string ToString()
        {
            return Name;
        }
    }
}
