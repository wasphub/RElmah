namespace RElmah.Common
{
    public enum DeltaType
    {
        Added,
        Removed,
        Updated
    }

    public static class Delta
    {
        public static Delta<T> Create<T>(T target, DeltaType type) where T : class
        {
            return new Delta<T>(target, type);
        }
    }

    public class Delta<T> where T : class
    {
        public Delta(T target, DeltaType type)
        {
            Target = target;
            Type = type;
        }

        public T Target { get; private set; }
        public DeltaType Type { get; private set; }
    }
}
