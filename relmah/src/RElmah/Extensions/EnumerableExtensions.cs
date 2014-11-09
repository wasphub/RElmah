using System;
using System.Collections.Generic;

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
    }
}
