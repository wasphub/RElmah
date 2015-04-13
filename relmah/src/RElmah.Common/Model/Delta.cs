using System.Runtime.Serialization;

namespace RElmah.Common.Model
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
            return Create(target, type, false);
        }

        public static Delta<T> Create<T>(T target, DeltaType type, bool fromBackend) where T : class
        {
            return new Delta<T>(target, type, fromBackend);
        }
    }

    public class Delta<T> : ISerializable
        where T : class
    {
        public Delta(T target, DeltaType type, bool fromBackend)
        {
            Target      = target;
            Type        = type;
            FromBackend = fromBackend;
        }

        public T Target { get; private set; }
        public DeltaType Type { get; private set; }
        public bool FromBackend { get; private set; }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Target", Target);
            info.AddValue("Type", Type);
            info.AddValue("FromBackend", FromBackend);
        }

        public Delta(SerializationInfo info, StreamingContext context)
        {
            Target      = (T)info.GetValue("Target", typeof(T));
            Type        = (DeltaType)info.GetValue("Type", typeof(DeltaType));
            FromBackend = (bool)info.GetValue("FromBackend", typeof(bool));
        }

        #endregion
    }
}
