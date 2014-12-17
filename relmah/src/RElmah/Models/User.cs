using System.Collections.Generic;
using System.Linq;
using RElmah.Extensions;

namespace RElmah.Models
{
    public class User
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
            return new User(Name, Tokens.EmptyIfNull().Concat(token.ToSingleton()));
        }

        public User RemoveToken(string token)
        {
            return new User(Name, Tokens.EmptyIfNull().Except(token.ToSingleton()));
        }
    }
}
