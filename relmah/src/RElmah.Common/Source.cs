using System.Runtime.Serialization;

namespace RElmah.Common
{
    public class Source : ISerializable
    {
        public static Source Create(string sourceId, string description)
        {
            return new Source(sourceId, description);
        }

        Source(string sourceId, string description)
        {
            SourceId = sourceId;
            Description = description;
        }

        public string SourceId { get; private set; }
        public string Description { get; private set; }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SourceId", SourceId);
            info.AddValue("Description", Description);
        }

        public Source(SerializationInfo info, StreamingContext context)
        {
            SourceId    = (string)info.GetValue("SourceId", typeof(string));
            Description = (string)info.GetValue("Description", typeof(string));
        }

        #endregion
    }
}
