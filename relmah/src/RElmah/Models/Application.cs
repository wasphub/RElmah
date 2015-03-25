using System.Runtime.Serialization;

namespace RElmah.Models
{
    public class Application : ISerializable
    {
        public static Application Create(string name)
        {
            return new Application(name);
        }

        Application(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public Application(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
        }

        #endregion
    }
}
