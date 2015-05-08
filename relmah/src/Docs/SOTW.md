    void Main()
    {
        var builder = new StateOfTheWorldBuilder<int, string, double, Error>((t, s) =>
        {
        });
    }

    public class Error { }

    public class Time<T>
    {
        public T Point { get; set; }
    }

    public class Span<T>
    {
        public T Size { get; set; }
    }

    public class StateOfTheWorld<TT, TG, TM, TS>
    {
        public Time<TT> Requested { get; set; }
        public Time<TT> Boundary  { get; set; }

        public IEnumerable<KeyValuePair<TG, TM>> Groups { get; set; }

        public IEnumerable<TS> Contributors { get; set; }

        Func<StateOfTheWorld<TT, TG, TM, TS>, IEnumerable<TS>, StateOfTheWorld<TT, TG, TM, TS>> Remove { get; set; }
    }

    public delegate StateOfTheWorld<TT, TG, TM, TS> StateOfTheWorldBuilder<TT, TG, TM, TS>(Time<TT> t, Span<TT> s);
