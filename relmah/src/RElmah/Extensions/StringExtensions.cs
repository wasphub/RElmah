using System;
using System.Linq;

namespace RElmah.Extensions
{
    public static class StringExtensions
    {
        public static bool IsTruthy(this string source)
        {
            var truthyValues = new []  { "true", "yes", "1" };
            return source != null && truthyValues.Select(v => source.Equals(v, StringComparison.OrdinalIgnoreCase)).Any(v => v);
        }
    }
}
