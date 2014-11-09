﻿using System.Collections.Generic;
using System.Linq;
using RElmah.Extensions;

namespace RElmah.Models.Configuration
{
    public class User
    {
        public static User Create(string name)
        {
            return new User(name);
        }

        User(string name)
        {
            Name = name;
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
            return new User(Name, Tokens.Concat(token.ToSingleton()));
        }
    }
}
