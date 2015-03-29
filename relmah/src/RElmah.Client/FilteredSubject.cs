using System;
using System.Reactive.Subjects;

namespace RElmah.Client
{
    class FilteredSubject<T> : ISubject<T>, IDisposable
    {
        private readonly ISubject<T> _subject;
        private readonly IObservable<T> _filtered;

        public FilteredSubject(Func<IObservable<T>, IObservable<T>> filter)
        {
            _subject = new Subject<T>();
            _filtered = filter != null ? filter(_subject) : _subject;
        }

        public void OnNext(T value)
        {
            _subject.OnNext(value);
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _filtered.Subscribe(observer);
        }

        public void Dispose()
        {
            var disposable = _subject as IDisposable;
            if (disposable != null) disposable.Dispose();

            disposable = _filtered as IDisposable;
            if (disposable != null) disposable.Dispose();
        }
    }
}