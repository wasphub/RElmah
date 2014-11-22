using System;
using System.Linq;
using RElmah.Grounding;

namespace RElmah.Host
{
    public static class GeneralExtensions
    {
        public static TR SafeCall<TT, TR>(this TT target, Func<TT, TR> call, Func<TR> @default, params Func<bool>[] guards) where TT : class
        {
            var checks =
                from g in guards
                let success = g.Catch()
                select success.HasValue && success.Value;

            return target != null && checks.TakeWhile(c => c).Count() == guards.Length 
                 ? call(target) 
                 : @default();
        }

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
