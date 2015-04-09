using System.Runtime.Serialization;

namespace RElmah.Models
{
    public class Source : ISerializable
    {
        public static Source Create(string sourceId)
        {
            return new Source(sourceId);
        }

        Source(string sourceId)
        {
            SourceId = sourceId;
        }

        public string SourceId { get; private set; }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SourceId", SourceId);
        }

        public Source(SerializationInfo info, StreamingContext context)
        {
            SourceId = (string)info.GetValue("SourceId", typeof(string));
        }

        #endregion
    }
}
