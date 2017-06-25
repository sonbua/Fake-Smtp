using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FakeSmtp.Helpers
{
    public class FixedSizeAndReversedOrderQueue<T> : IEnumerable<T>
    {
        private readonly int _size;
        private readonly List<T> _items;
        private readonly object _lockObject = new object();

        public FixedSizeAndReversedOrderQueue(int size)
        {
            _size = size;
            _items = new List<T>();
        }

        public FixedSizeAndReversedOrderQueue(int size, FixedSizeAndReversedOrderQueue<T> existingQueue)
        {
            _size = size;
            _items = new List<T>(existingQueue._items);

            if (_items.Count > _size)
            {
                _items.RemoveRange(_size, _items.Count - _size);
            }
        }

        public int Count => _items.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lockObject)
            {
                return _items.GetEnumerator();
            }
        }

        public void Enqueue(T item)
        {
            lock (_lockObject)
            {
                if (_items.Count == _size)
                {
                    _items.RemoveAt(_items.Count - 1);
                }

                _items.Insert(0, item);
            }
        }

        public T Dequeue()
        {
            lock (_lockObject)
            {
                var dequeuedItem = _items.Last();

                _items.RemoveAt(_items.Count - 1);

                return dequeuedItem;
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _items.Clear();
            }
        }
    }
}