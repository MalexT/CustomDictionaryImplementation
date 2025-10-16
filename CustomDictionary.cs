using System.Collections;

namespace CustomDictionaryImplementation
{
    internal class CustomDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int DefaultCapacity = 16;
        private const float MaxLoadFactor = 0.75f;

        private SinglyLinkedList<KeyValuePair<TKey, TValue>>[] _buckets;
        private int _count;
        private readonly IEqualityComparer<TKey> _comparer;

        public int Count => _count;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public CustomDictionary(int capacity = DefaultCapacity, IEqualityComparer<TKey>? comparer = null)
        {
            if (capacity <= 0)
            {
                capacity = DefaultCapacity;
            }

            _buckets = new SinglyLinkedList<KeyValuePair<TKey, TValue>>[capacity];
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
            {
                throw new ArgumentException("An element with the same key already exists.");
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            EnsureTheresCapacityBeforeInsert();
            int idx = GetBucketIndex(key);

            SinglyLinkedList<KeyValuePair<TKey, TValue>> bucket = _buckets[idx];

            if (bucket == null)
            {
                bucket = new SinglyLinkedList<KeyValuePair<TKey, TValue>>();
                _buckets[idx] = bucket;
            }

            for (var node = bucket.First; node != null; node = node.Next)
            {
                var kv = node.Data;
                if (_comparer.Equals(kv.Key, key))
                {
                    return false; // Key already exists
                }
            }

            bucket.Add(new KeyValuePair<TKey, TValue>(key, value));
            _count++;

            return true;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            int idx = GetBucketIndex(key);
            var bucket = _buckets[idx];

            if (bucket == null)
            {
                return false;
            }

            // Remove first node where kv.Key == key
            bool removed = bucket.Remove(kv => _comparer.Equals(kv.Key, key));
            if (removed)
            {
                _count--;
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            value = default;

            int idx = GetBucketIndex(key);
            var bucket = _buckets[idx];

            if (bucket != null)
            {
                for (var node = bucket.First; node != null; node = node.Next)
                {
                    var kv = node.Data;
                    if (_comparer.Equals(kv.Key, key))
                    {
                        value = kv.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        private int GetBucketIndex(TKey key)
        {
            int hash = Math.Abs(_comparer.GetHashCode(key));
            return hash % _buckets.Length;
        }

        public TValue Get(TKey key)
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException("Key not found.");
        }

        private void EnsureTheresCapacityBeforeInsert()
        {
            if ((_count + 1) > _buckets.Length * MaxLoadFactor)
            {
                Resize(_buckets.Length * 2);
            }
        }

        private void Resize(int newSize)
        {
            var newBuckets = new SinglyLinkedList<KeyValuePair<TKey, TValue>>[newSize];

            foreach (var pair in this)
            {
                int hash = Math.Abs(_comparer.GetHashCode(pair.Key));
                int idx = hash % newSize;

                if (newBuckets[idx] == null)
                {
                    newBuckets[idx] = new SinglyLinkedList<KeyValuePair<TKey, TValue>>();
                }

                newBuckets[idx].Add(pair);
            }
            _buckets = newBuckets;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in _buckets)
            {
                if (bucket == null)
                {
                    continue;
                }

                foreach (var kv in bucket)
                {
                    yield return kv;
                }
            }
        }
    }
}
