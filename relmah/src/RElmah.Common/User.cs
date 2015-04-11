using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using RElmah.Common.Extensions;

namespace RElmah.Common
{
    public class User : ISerializable
    {
        public static User Create(string name)
        {
            return new User(name);
        }

        User(string name) : this(name, new string[] { })
        {
        }

        User(string name, IEnumerable<string> tokens)
        {
            Name = name;
            Tokens = tokens;
        }

        public string Name { get; private set; }
        public IEnumerable<string> Tokens { get; private set; }

        public User AddToken(string token)
        {
            return new User(Name, Enumerable.Concat<string>(Tokens.EmptyIfNull(), token.ToSingleton()));
        }

        public User RemoveToken(string token)
        {
            return new User(Name, Enumerable.Except<string>(Tokens.EmptyIfNull(), token.ToSingleton()));
        }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public User(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
        }

        #endregion
    }
}
