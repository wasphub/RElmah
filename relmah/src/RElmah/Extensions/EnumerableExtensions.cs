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
                }
            }

            return source;
        }

        public static IEnumerable<T> ToSingleton<T>(this T source)
        {
            return new[] {source};
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
