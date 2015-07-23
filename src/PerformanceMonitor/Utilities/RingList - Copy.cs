using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace PerformanceMonitor.Utilities
{
    public class RingList<T> : INotifyCollectionChanged, IList<T>
    {
        #region Fields

        private readonly int _capacity;
        private T[] _array;
        private int _startIndex;
        private int _count;

        #endregion

        #region Constructor

        public RingList(int capacity)
        {
            _capacity = capacity;
            _array = new T[capacity];
        }

        #endregion

        #region Properties

        public int Capacity
        {
            get { return _capacity; }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged()
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            if (handler != null) handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region IList<T> Members

        public void Add(T item)
        {
            int index = (_startIndex + _count) % _capacity;
            if (_startIndex + _count >= _capacity)
            {
                _startIndex++;
            }
            else
            {
                _count++;
            }

            _array[index] = item;

            OnCollectionChanged();
        }

        public T this[int index]
        {
            get { return _array[(_startIndex + index) % _capacity]; }
            set
            {
                _array[(_startIndex + index) % _capacity] = value;
                OnCollectionChanged();
            }
        }

        public int IndexOf(T item)
        {
            int index = Array.IndexOf(_array, item);

            if (index == -1)
                return -1;

            return (index - _startIndex + _count) % _capacity;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T> Members

        public void Clear()
        {
            _count = 0;
            _startIndex = 0;
            _array = new T[_capacity];

            OnCollectionChanged();
        }

        public int Count
        {
            get { return _count; }
        }

        public bool Contains(T item)
        {
            return Array.IndexOf(_array, item) > -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
