using System;

namespace RElmah.Extensions
{
    public static class BooleanExtensions
    {
        public static T Then<T>(this bool source, Func<T> creator)
        {
            return source ? creator() : default(T);
        }
    }
}