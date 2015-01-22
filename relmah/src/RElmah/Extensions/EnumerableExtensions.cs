using System;
using System.Collections.Generic;
using System.Linq;

namespace RElmah.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
            {
                var e = source.GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                    yield return e.Current;
                }
            }
        }

        public static void Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var e in source)
                action(e);
        }

        public static IEnumerable<T> ToSingleton<T>(this T source)
        {
            return new[] {source};
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<KeyValuePair<int, T>> Index<T>(this IEnumerable<T> source)
        {
            var e = source.GetEnumerator();
            var i = 0;
            while (e.MoveNext())
            {
                yield return new KeyValuePair<int, T>(i++, e.Current);
            }
        }
    }
}
