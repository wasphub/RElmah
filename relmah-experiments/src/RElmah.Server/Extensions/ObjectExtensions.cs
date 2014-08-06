using System.Collections.Generic;
using System.Linq;

namespace RElmah.Server.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToSingleton<T>(this T source)
        {
            return Enumerable.Repeat(source, 1);
        }
    }
}
