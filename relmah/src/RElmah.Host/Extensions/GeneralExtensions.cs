using System;
using RElmah.Foundation;

namespace RElmah.Host.Extensions
{
    public static class GeneralExtensions
    {
        public static ValueOrError<T> Catch<T>(this Func<T> call)
        {
            T result;

            try
            {
                result = call();
            }
            catch (Exception ex)
            {
                return new ValueOrError<T>(ex);
            }

            return new ValueOrError<T>(result);
        }
    }
}
