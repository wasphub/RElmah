using System;
using System.Threading;

namespace RElmah.Foundation
{
    public class LayeredDisposable : IDisposable
    {
        private IReleasable _underlying;

        LayeredDisposable(IDisposable underlying)
        {
            if (underlying == null) throw new ArgumentNullException();

            _underlying = new OriginalReleasable(underlying);
        }

        public static LayeredDisposable Create(IDisposable underlying)
        {
            return new LayeredDisposable(underlying);
        }

        static IReleasable Wrap(IReleasable underlying)
        {
            return new ReleasableWrapper(underlying);
        }

        public void Wrap()
        {
            Interlocked.Exchange(ref _underlying, Wrap(_underlying));
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _underlying, _underlying.Release());
        }

        public bool IsDisposed
        {
            get { return _underlying == null; }
        }

        interface IReleasable
        {
            IReleasable Release();
        }

        class OriginalReleasable : IReleasable
        {
            private readonly IDisposable _original;

            public OriginalReleasable(IDisposable original)
            {
                _original = original;
            }

            public IReleasable Release()
            {
                _original.Dispose();
                return null;
            }
        }

        class ReleasableWrapper : IReleasable
        {
            private readonly IReleasable _wrapped;

            public ReleasableWrapper(IReleasable wrapped)
            {
                _wrapped = wrapped;
            }

            public IReleasable Release()
            {
                return _wrapped;
            }
        }
    }
}
