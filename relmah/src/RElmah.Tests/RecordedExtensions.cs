using Microsoft.Reactive.Testing;

namespace RElmah.Tests
{
    public static class RecordedExtensions
    {
        public static Recorded<T> RecordAt<T>(this T source, long t)
        {
            return new Recorded<T>(t, source);
        }
    }
}