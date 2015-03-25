using System.Runtime.Serialization;

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

    public class Delta<T> : ISerializable
        where T : class
    {
        public Delta(T target, DeltaType type)
        {
            Target = target;
            Type = type;
        }

        public T Target { get; private set; }
        public DeltaType Type { get; private set; }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Target", Target);
            info.AddValue("Type", Type);
        }

        public Delta(SerializationInfo info, StreamingContext context)
        {
            Target = (T)info.GetValue("Target", typeof(T));
            Type   = (DeltaType)info.GetValue("Type", typeof(DeltaType));
        }

        #endregion
    }
}
