using System;

namespace PerformanceMonitor.Controls.QuickCharts
{
    /// <summary>
    /// Represents arguments for event raised when Path related properties change.
    /// </summary>
    public class DataPathEventArgs : EventArgs
    {
        /// <summary>
        /// Instantiates class with specified new path.
        /// </summary>
        /// <param name="newPath"></param>
        public DataPathEventArgs(string newPath)
        {
            NewPath = newPath;
        }

        /// <summary>
        /// Gets or sets new path.
        /// </summary>
        public string NewPath { get; set; }
    }

    public class ValueEventArgs<T> : EventArgs
    {
        public object Item { get; set; }

        public string Path { get; set; }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsValueSet = true;
            }
        }

        public bool IsValueSet { get; private set; }
    }
}
