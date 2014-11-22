using System;
using System.Linq;

namespace RElmah.Host
{
    public static class GeneralExtensions
    {
        public static TR SafeCall<TT, TR>(this TT target, Func<TT, TR> call, Func<TR> @default, params Func<bool>[] guards)
        {
            var checks =
                from g in guards
                let success = g()
                select success;

            var ok = checks.TakeWhile(c => c).Count() == guards.Length;
            return ok ? call(target) : @default();
        }
    }
}
