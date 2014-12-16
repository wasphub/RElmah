using System;
using System.Collections.Generic;
using System.Linq;

namespace RElmah.Extensions
{
    public static class DictionaryExtensions
    {
        public static TR Get<TK, TR>(this IDictionary<TK, TR> source, TK key, TR defaultValue)
        {
            return source.ContainsKey(key) ? source[key] : defaultValue;
        }

        public static TR Get<TK, TR>(this IReadOnlyDictionary<TK, TR> source, TK key, TR defaultValue)
        {
            return source.ContainsKey(key) ? source[key] : defaultValue;
        }

        public static TR Get<TK, TR>(this IDictionary<TK, TR> source, TK key) where TR : class
        {
            return source.ContainsKey(key) ? source[key] : null;
        }

        public static TR Get<TK, TR>(this IReadOnlyDictionary<TK, TR> source, TK key) where TR : class
        {
            return source.ContainsKey(key) ? source[key] : null;
        }

        public static KeyValuePair<TK, TV> AsKeyFor<TK, TV>(this TK key, TV value)
        {
            return new KeyValuePair<TK, TV>(key, value);
        }

        public static IDictionary<TK, TR> ToDictionary<TK, TR>(this IEnumerable<KeyValuePair<TK, TR>> source)
        {
            return source.ToDictionary(k => k.Key, v => v.Value);
        }

        public static IEnumerable<KeyValuePair<TK, TR>> Select<TK, TV, TR>(this IEnumerable<KeyValuePair<TK, TV>> source, Func<TV, TR> selector)
        {
            return from kvp in source select new KeyValuePair<TK, TR>(kvp.Key, selector(kvp.Value));
        }
    }
}
