using System;
using RElmah.Foundation;

namespace RElmah.Extensions
{
    public static class DisposableExtensions
    {
        public static LayeredDisposable ToLayeredDisposable(this IDisposable underlying)
        {
            return LayeredDisposable.Create(underlying);
        }
    }
}