using System.Collections.ObjectModel;

namespace PerformanceMonitor.Utilities
{
    /// <summary>
    /// A version of <see cref="ObservableCollection{T}"/> which on call to Clear() removes items one-by-one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DiscreteClearObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Removes items from the collection one-by-one.
        /// </summary>
        protected override void ClearItems()
        {
            for (var i = Count - 1; i >= 1; i--)
            {
                RemoveAt(i);
            }
        }
    }
}