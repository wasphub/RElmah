using System.Collections.Generic;

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
    }
}
