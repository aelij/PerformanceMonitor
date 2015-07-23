using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PerformanceMonitor.Utilities
{
    public class RingList<T> : INotifyCollectionChanged, IList<T>
    {
        #region Fields

        private T[] _array;
        private int _startIndex;

        #endregion

        #region Constructor

        public RingList(int capacity)
        {
            _array = new T[capacity];
        }

        #endregion
        
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var handler = CollectionChanged;
            handler?.Invoke(this, args);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        #endregion

        #region IList<T> Members

        public void Add(T item)
        {
            var index = _startIndex++;
            if (_startIndex >= _array.Length)
            {
                _startIndex = _startIndex % _array.Length;
            }
        
            var oldItem = _array[index];
            _array[index] = item;

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        }


        public int IndexOf(T item)
        {
            return Array.IndexOf(_array, item);
        }

        T IList<T>.this[int index]
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T> Members

        public void Clear()
        {
            _startIndex = 0;
            _array = new T[_array.Length];

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public int Count => _array.Length;

        public bool Contains(T item)
        {
            return Array.IndexOf(_array, item) > -1;
        }

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
