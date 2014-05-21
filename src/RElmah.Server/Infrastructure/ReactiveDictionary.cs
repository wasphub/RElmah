using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using RElmah.Domain;
using RElmah.Server.Extensions;

namespace RElmah.Server.Infrastructure
{
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
            OnNext(new UpdateEntry<TValue>(item.Value.ToSingleton(), UpdateEntryType.Create));
        }

        public void Clear()
        {
            OnNext(new UpdateEntry<TValue>(_dictionary.Values, UpdateEntryType.Remove));
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
            OnNext(new UpdateEntry<TValue>(item.Value.ToSingleton(), UpdateEntryType.Remove));
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
            OnNext(new UpdateEntry<TValue>(value.ToSingleton(), UpdateEntryType.Create));
        }

        public bool Remove(TKey key)
        {
            OnNext(new UpdateEntry<TValue>(_dictionary[key].ToSingleton(), UpdateEntryType.Remove));
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
                OnNext(new UpdateEntry<TValue>(value.ToSingleton(), updateEntryType));
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