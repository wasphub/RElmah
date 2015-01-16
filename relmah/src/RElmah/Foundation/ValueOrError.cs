using System;

namespace RElmah.Foundation
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

        public static ValueOrError<T> Null<T>()
        {
            return new ValueOrError<T>(new ArgumentNullException());
        }

        public static ValueOrError<T> ToValueOrError<T>(this T value)
        {
            return typeof (T).IsValueType 
                   ? !default(T).Equals(value) 
                     ? new ValueOrError<T>(value) : new ValueOrError<T>(new ArgumentNullException())
                   : value != null 
                     ? new ValueOrError<T>(value) : new ValueOrError<T>(new ArgumentNullException());
        }
    }
}
