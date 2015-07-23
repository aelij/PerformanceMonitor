using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using PerformanceMonitor.Annotations;

namespace PerformanceMonitor.Utilities
{
    public class CollectionProxy<TItem> : IEnumerable<TItem>
    {
        private readonly object _lock;
        private readonly ObservableCollection<TItem> _collection;

        public ICollectionView CollectionView { get; private set; }

        public CollectionProxy()
        {
            _lock = new object();
            _collection = new ObservableCollection<TItem>();
            BindingOperations.EnableCollectionSynchronization(_collection, _lock);
            CollectionView = new CollectionViewSource { Source = _collection }.View;
        }

        public void Load([NotNull] IEnumerable<TItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            lock (_lock)
            {
                var newItems = items.ToArray();
                foreach (var oldItem in _collection.Where(oldItem => !newItems.Contains(oldItem)).ToArray())
                {
                    RemoveInternal(oldItem);
                }
                foreach (var newItem in newItems.Where(t => !_collection.Contains(t)))
                {
                    AddInternal(newItem);
                }
            }
        }

        public TItem this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _collection[index];
                }
            }
        }

        public void Remove(TItem item)
        {
            lock (_lock)
            {
                RemoveInternal(item);
            }
        }

        private void RemoveInternal(TItem item)
        {
            _collection.Remove(item);
            OnItemRemoved(item);
        }

        public void Add(TItem item)
        {
            lock (_lock)
            {
                AddInternal(item);
            }
        }

        private void AddInternal(TItem item)
        {
            _collection.Add(item);
            OnItemAdded(item);
        }

        public TItem[] ToArray()
        {
            lock (_lock)
            {
                return _collection.ToArray();
            }
        }

        protected virtual void OnItemRemoved([NotNull] TItem item)
        {
        }

        protected virtual void OnItemAdded([NotNull] TItem item)
        {
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class KeyedCollectionProxy<TKey, TItem> : CollectionProxy<TItem>
    {
        private readonly Func<TItem, TKey> _keySelector;
        private readonly Dictionary<TKey, TItem> _dictionary;

        public KeyedCollectionProxy([NotNull] Func<TItem, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            _keySelector = keySelector;
            _dictionary = new Dictionary<TKey, TItem>();
        }

        public bool TryGetItem(TKey key, out TItem item)
        {
            return _dictionary.TryGetValue(key, out item);
        }

        protected override void OnItemAdded(TItem item)
        {
            base.OnItemAdded(item);
            _dictionary.Add(_keySelector(item), item);
        }

        protected override void OnItemRemoved(TItem item)
        {
            base.OnItemRemoved(item);
            _dictionary.Remove(_keySelector(item));
        }
    }
}
