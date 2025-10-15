using System.Collections;

namespace CustomDictionaryImplementation
{
    internal class CustomDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int DefaultCapacity = 16;
        private const float MaxLoadFactor = 0.75f;

        private LinkedList<KeyValuePair<TKey, TValue>>[] _buckets;
        private int _count;
        private readonly IEqualityComparer<TKey> _comparer;

        public int Count => _count;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public CustomDictionary(int capacity = DefaultCapacity, IEqualityComparer<TKey> comparer = null)
        {
            if (capacity <= 0)
            {
                capacity = DefaultCapacity;
            }

            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[capacity];
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

            LinkedList<KeyValuePair<TKey, TValue>> bucket = _buckets[idx];

            if (bucket == null)
            {
                bucket = new LinkedList<KeyValuePair<TKey, TValue>>();
                _buckets[idx] = bucket;
            }

            for (var node = bucket.First; node != null; node = node.Next)
            {
                if (_comparer.Equals(node.Value.Key, key))
                {
                    return false; // Key already exists
                }
            }

            bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
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

            for (var node = bucket.First; node != null; node = node.Next)
            {
                if (_comparer.Equals(node.Value.Key, key))
                {
                    bucket.Remove(node);
                    _count--;
                    return true;
                }
            }

            return false;
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
                    if (_comparer.Equals(node.Value.Key, key))
                    {
                        value = node.Value.Value;
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
            var newBuckets = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];

            foreach (var pair in this)
            {
                int hash = Math.Abs(_comparer.GetHashCode(pair.Key));
                int idx = hash % newSize;

                if (newBuckets[idx] == null)
                {
                    newBuckets[idx] = new LinkedList<KeyValuePair<TKey, TValue>>();
                }

                newBuckets[idx].AddLast(pair);
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
