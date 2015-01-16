using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RElmah.Foundation
{
    public class AtomicImmutableList<T> : IImmutableList<T>
    {
        private readonly ConcurrentReference<IImmutableList<T>> _backed =
            new ConcurrentReference<IImmutableList<T>>(ImmutableList<T>.Empty);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _backed.Value.GetEnumerator();
        }

        public int Count { get { return _backed.Value.Count; } }


        public T this[int index]
        {
            get { return _backed.Value[index]; }
        }

        public IImmutableList<T> Clear()
        {
            return _backed.Mutate(p => p.Clear());
        }

        public int IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return _backed.Value.IndexOf(item, index, count, equalityComparer);
        }

        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return _backed.Value.LastIndexOf(item, index, count, equalityComparer);
        }

        public IImmutableList<T> Add(T value)
        {
            return _backed.Mutate(p => p.Add(value));
        }

        public IImmutableList<T> AddRange(IEnumerable<T> items)
        {
            return _backed.Mutate(p => p.AddRange(items));
        }

        public IImmutableList<T> Insert(int index, T element)
        {
            return _backed.Mutate(p => p.Insert(index, element));
        }

        public IImmutableList<T> InsertRange(int index, IEnumerable<T> items)
        {
            return _backed.Mutate(p => p.InsertRange(index, items));
        }

        public IImmutableList<T> Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            return _backed.Mutate(p => p.Remove(value, equalityComparer));
        }

        public IImmutableList<T> RemoveAll(Predicate<T> match)
        {
            return _backed.Mutate(p => p.RemoveAll(match));
        }

        public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            return _backed.Mutate(p => p.RemoveRange(items, equalityComparer));
        }

        public IImmutableList<T> RemoveRange(int index, int count)
        {
            return _backed.Mutate(p => p.RemoveRange(index, count));
        }

        public IImmutableList<T> RemoveAt(int index)
        {
            return _backed.Mutate(p => p.RemoveAt(index));
        }

        public IImmutableList<T> SetItem(int index, T value)
        {
            return _backed.Mutate(p => p.SetItem(index, value));
        }

        public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            return _backed.Mutate(p => p.Replace(oldValue, newValue, equalityComparer));
        }
    }
}