using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using RElmah.Common;
using RElmah.Server.Extensions;

namespace RElmah.Server.Infrastructure
{
    public class ReactiveDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue>                      _dictionary = new ConcurrentDictionary<TKey, TValue>();
        private readonly ISubject<Delta<TValue>, Delta<TValue>> _source     = Subject.Synchronize(new Subject<Delta<TValue>>());

        public IObservable<Delta<TValue>> Deltas
        {
            get { return _source; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item);
            _source.OnNext(new Delta<TValue>(item.Value.ToSingleton(), DeltaType.Create));
        }

        public void Clear()
        {
            _source.OnNext(new Delta<TValue>(_dictionary.Values, DeltaType.Remove));
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _source.OnNext(new Delta<TValue>(item.Value.ToSingleton(), DeltaType.Remove));
            return _dictionary.Remove(item);
        }

        public int Count { get { return _dictionary.Count; } }
        public bool IsReadOnly { get { return false; } }
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _source.OnNext(new Delta<TValue>(value.ToSingleton(), DeltaType.Create));
        }

        public bool Remove(TKey key)
        {
            _source.OnNext(new Delta<TValue>(_dictionary[key].ToSingleton(), DeltaType.Remove));
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set
            {
                var updateEntryType = _dictionary.ContainsKey(key) 
                    ? DeltaType.Update 
                    : DeltaType.Create;
                _dictionary[key] = value;
                _source.OnNext(new Delta<TValue>(value.ToSingleton(), updateEntryType));
            }
        }

        public ICollection<TKey> Keys { get { return _dictionary.Keys; } }
        public ICollection<TValue> Values { get { return _dictionary.Values; } }        
    }
}