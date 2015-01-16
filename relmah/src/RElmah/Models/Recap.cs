using System.Collections.Generic;

namespace RElmah.Models
{
    public class Recap
    {
        public IEnumerable<Application> Apps { get; private set; }

        public Recap(IEnumerable<Application> apps)
        {
            Apps = apps;
        }

        public class Application
        {
            public string Name { get; private set; }
            public IEnumerable<Type> Types { get; private set; }

            public Application(string type, IEnumerable<Type> types)
            {
                Name = type;
                Types = types;
            }
        }

        public class Type
        {
            public string Name { get; private set; }
            public int Measure { get; private set; }

            public Type(string name, int measure)
            {
                Name = name;
                Measure = measure;
            }
        }
    }
}