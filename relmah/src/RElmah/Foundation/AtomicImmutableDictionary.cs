using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RElmah.Foundation
{
    public class AtomicImmutableDictionary<TK, TV> : IImmutableDictionary<TK, TV>
    {
        private readonly ConcurrentReference<IImmutableDictionary<TK, TV>> _backed =
            new ConcurrentReference<IImmutableDictionary<TK, TV>>(ImmutableDictionary<TK, TV>.Empty);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return _backed.Value.GetEnumerator();
        }

        public int Count { get { return _backed.Value.Count; } }

        public bool ContainsKey(TK key)
        {
            return _backed.Value.ContainsKey(key);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return _backed.Value.TryGetValue(key, out value);
        }

        public TV this[TK key]
        {
            get { return _backed.Value[key]; }
        }

        public IEnumerable<TK> Keys { get { return _backed.Value.Keys; } }
        public IEnumerable<TV> Values { get { return _backed.Value.Values; } }

        public IImmutableDictionary<TK, TV> Clear()
        {
            return _backed.Mutate(p => p.Clear());
        }

        public IImmutableDictionary<TK, TV> Add(TK key, TV value)
        {
            return _backed.Mutate(p => p.Add(key, value));
        }

        public IImmutableDictionary<TK, TV> AddRange(IEnumerable<KeyValuePair<TK, TV>> pairs)
        {
            return _backed.Mutate(p => p.AddRange(pairs));
        }

        public IImmutableDictionary<TK, TV> SetItem(TK key, TV value)
        {
            return _backed.Mutate(p => p.SetItem(key, value));
        }

        public IImmutableDictionary<TK, TV> SetItems(IEnumerable<KeyValuePair<TK, TV>> items)
        {
            return _backed.Mutate(p => p.SetItems(items));
        }

        public IImmutableDictionary<TK, TV> RemoveRange(IEnumerable<TK> keys)
        {
            return _backed.Mutate(p => p.RemoveRange(keys));
        }

        public IImmutableDictionary<TK, TV> Remove(TK key)
        {
            return _backed.Mutate(p => p.Remove(key));
        }

        public bool Contains(KeyValuePair<TK, TV> pair)
        {
            return _backed.Value.Contains(pair);
        }

        public bool TryGetKey(TK equalKey, out TK actualKey)
        {
            return _backed.Value.TryGetKey(equalKey, out actualKey);
        }
    }
}
