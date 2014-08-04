using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using RElmah.Common;
using RElmah.Server.Extensions;

namespace RElmah.Server.Infrastructure
{
    public class ReactiveDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISubject<Operation<TValue>>
    {
        readonly IDictionary<TKey, TValue> _dictionary = new ConcurrentDictionary<TKey, TValue>();
        private readonly ISubject<Operation<TValue>, Operation<TValue>> _source = Subject.Synchronize(new Subject<Operation<TValue>>());

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
            OnNext(new Operation<TValue>(item.Value.ToSingleton(), OperationType.Create));
        }

        public void Clear()
        {
            OnNext(new Operation<TValue>(_dictionary.Values, OperationType.Remove));
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
            OnNext(new Operation<TValue>(item.Value.ToSingleton(), OperationType.Remove));
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
            OnNext(new Operation<TValue>(value.ToSingleton(), OperationType.Create));
        }

        public bool Remove(TKey key)
        {
            OnNext(new Operation<TValue>(_dictionary[key].ToSingleton(), OperationType.Remove));
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
                    ? OperationType.Update 
                    : OperationType.Create;
                _dictionary[key] = value;
                OnNext(new Operation<TValue>(value.ToSingleton(), updateEntryType));
            }
        }

        public ICollection<TKey> Keys { get { return _dictionary.Keys; } }
        public ICollection<TValue> Values { get { return _dictionary.Values; } }


        public void OnNext(Operation<TValue> value)
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

        public IDisposable Subscribe(IObserver<Operation<TValue>> observer)
        {
            return _source.Subscribe(observer);
        }
    }
}