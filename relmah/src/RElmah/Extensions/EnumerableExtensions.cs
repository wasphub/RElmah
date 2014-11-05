using System;
using System.Collections.Generic;

namespace RElmah.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) yield break;

            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }
    }
}
