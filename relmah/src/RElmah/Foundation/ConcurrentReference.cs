using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace RElmah.Foundation
{
    public class ConcurrentReference<T> where T : class
    {
        private volatile T _value;

        public ConcurrentReference(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }

        public T Mutate(Func<T, T> mutator)
        {
            Contract.Requires(mutator != null);

            var original = _value;
            while (true)
            {
                var mutated = mutator(_value);

                // ReSharper disable CSharpWarnings::CS0420
                var current = Interlocked.CompareExchange(ref _value, mutated, original);
                // ReSharper restore CSharpWarnings::CS0420

                if (current == original)
                    return original;

                original = current;
            }
        }
    }
}
