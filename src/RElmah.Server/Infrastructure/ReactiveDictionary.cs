using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace RElmah.Server.Infrastructure
{
    public enum UpdateEntryType
    {
        Create,
        Update,
        Remove
    }
    public class UpdateEntry<T>
    {
        public UpdateEntry(T entry, UpdateEntryType type)
        {
            Entry = entry;
            Type = type;
        }

        public T Entry { get; private set; }
        public UpdateEntryType Type { get; private set; }
    }

    public class ReactiveDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISubject<UpdateEntry<TValue>>
    {
        readonly IDictionary<TKey, TValue> _dictionary = new ConcurrentDictionary<TKey, TValue>();
        private readonly Subject<UpdateEntry<TValue>> _source = new Subject<UpdateEntry<TValue>>();

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
            OnNext(new UpdateEntry<TValue>(item.Value, UpdateEntryType.Create));
        }

        public void Clear()
        {
            foreach (var i in _dictionary.Values)
                OnNext(new UpdateEntry<TValue>(i, UpdateEntryType.Remove));
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
            OnNext(new UpdateEntry<TValue>(item.Value, UpdateEntryType.Remove));
            return _dictionary.Remove(item);
        }

        public int Count { get { return _dictionary.Count; } }
        public bool IsReadOnly { get { return _dictionary.IsReadOnly; } }
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            OnNext(new UpdateEntry<TValue>(value, UpdateEntryType.Create));
        }

        public bool Remove(TKey key)
        {
            OnNext(new UpdateEntry<TValue>(_dictionary[key], UpdateEntryType.Remove));
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
                    ? UpdateEntryType.Update 
                    : UpdateEntryType.Create;
                _dictionary[key] = value;
                OnNext(new UpdateEntry<TValue>(value, updateEntryType));
            }
        }

        public ICollection<TKey> Keys { get { return _dictionary.Keys; } }
        public ICollection<TValue> Values { get { return _dictionary.Values; } }


        public void OnNext(UpdateEntry<TValue> value)
        {
            _source.OnNext(value);
        }

        public void OnError(Exception error)
        {
            _source.OnError(error);
        }

        public void OnCompleted()
        {
            _source.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<UpdateEntry<TValue>> observer)
        {
            return _source.Subscribe(observer);
        }
    }
}