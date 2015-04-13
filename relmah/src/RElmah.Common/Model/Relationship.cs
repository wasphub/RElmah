using System.Runtime.Serialization;

namespace RElmah.Common.Model
{
    public static class Relationship
    {
        public static Relationship<TP, TS> Create<TP, TS>(TP primary, TS secondary)
        {
            return new Relationship<TP, TS>(primary, secondary);
        }
    }

    public class Relationship<TP, TS> : ISerializable
    {
        public Relationship(TP primary, TS secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }

        public TP Primary { get; private set; }
        public TS Secondary { get; private set; }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Primary", Primary);
            info.AddValue("Secondary", Secondary);
        }

        public Relationship(SerializationInfo info, StreamingContext context)
        {
            Primary   = (TP)info.GetValue("Primary", typeof(TP));
            Secondary = (TS)info.GetValue("Secondary", typeof(TS));
        }

        #endregion
    }
}
