using System.Collections.Generic;

namespace RElmah.Server.Extensions
{
    public static class DictionaryExtensions
    {
        public static TR Get<TK, TR>(this IDictionary<TK, TR> source, TK key, TR defaultValue)
        {
            return source.ContainsKey(key) ? source[key] : defaultValue;
        }
    }
}