using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FakeSmtp.Models;

namespace FakeSmtp.Helpers
{
    public class Inbox : IEnumerable<Email>
    {
        public static Inbox Empty = new Inbox();

        private readonly int _size;
        private readonly FixedSizeAndReversedOrderQueue<Email> _emails;
        private readonly object _lockObject = new object();

        private Inbox()
        {
            _emails = new FixedSizeAndReversedOrderQueue<Email>(0);
        }

        public Inbox(int size) : this(size, existingInbox: Empty)
        {
        }

        public Inbox(int size, Inbox existingInbox)
        {
            _size = size;
            _emails = new FixedSizeAndReversedOrderQueue<Email>(_size, existingInbox._emails);

            while (_emails.Count > _size)
            {
                _emails.Dequeue();
            }
        }

        public int Count => _emails.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Email> GetEnumerator()
        {
            lock (_lockObject)
            {
                return _emails.GetEnumerator();
            }
        }

        public void Receive(Email email)
        {
            lock (_lockObject)
            {
                email.Id = _emails.Count > 0 ? _emails.First().Id + 1 : 1;

                _emails.Enqueue(email);
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _emails.Clear();
            }
        }
    }
}