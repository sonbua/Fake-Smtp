using System.Collections;
using System.Collections.Generic;

namespace FakeSmtp.Helpers
{
    public class FixedSizeAndReversedOrderQueue<T> : IEnumerable<T>
    {
        private readonly List<T> _items;
        private readonly object _lockObject = new object();

        public FixedSizeAndReversedOrderQueue(int size)
        {
            Size = size;
            _items = new List<T>();
        }

        public FixedSizeAndReversedOrderQueue(int size, FixedSizeAndReversedOrderQueue<T> existingQueue)
        {
            Size = size;
            _items = new List<T>(existingQueue._items);

            while (_items.Count > Size)
            {
                _items.RemoveRange(Size, _items.Count - Size);
            }
        }

        public int Size { get; }

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

        public void Insert(T item)
        {
            lock (_lockObject)
            {
                if (_items.Count == Size)
                {
                    _items.RemoveAt(_items.Count - 1);
                }

                _items.Insert(0, item);
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