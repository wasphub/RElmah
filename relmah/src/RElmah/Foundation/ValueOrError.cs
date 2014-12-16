using System;

namespace RElmah.Grounding
{
    public class ValueOrError<T>
    {
        public T Value { get; private set; }
        public Exception Error { get; private set; }

        public ValueOrError(T value)
        {
            Value = value;
        }

        public ValueOrError(Exception error)
        {
            Error = error;
        }

        public bool HasValue { get { return Error == null; } }
    }

    public static class ValueOrError
    {
        public static ValueOrError<T> Create<T>(T value)
        {
            return new ValueOrError<T>(value);
        }

        public static ValueOrError<T> Create<T>(Exception error)
        {
            return new ValueOrError<T>(error);
        }
    }
}
