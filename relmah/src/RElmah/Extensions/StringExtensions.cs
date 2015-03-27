using System;
using System.Linq;

namespace RElmah.Extensions
{
    public static class StringExtensions
    {
        public static bool IsTruthy(this string source, bool def = false)
        {
            if (string.IsNullOrWhiteSpace(source)) return def;

            var truthyValues = new []  { "true", "yes", "1" };
            return truthyValues.Select(v => source.Equals(v, StringComparison.OrdinalIgnoreCase)).Any(v => v);
        }
    }
}
