using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RElmah.Common
{
    public class Recap : ISerializable
    {
        public IEnumerable<Application> Apps { get; private set; }
        public DateTime When { get; private set; }

        public Recap(DateTime when, IEnumerable<Application> apps)
        {
            When = when;
            Apps = apps;
        }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("When", When);
            info.AddValue("Apps", Apps);
        }

        public Recap(SerializationInfo info, StreamingContext context)
        {
            When = (DateTime)info.GetValue("When", typeof(DateTime));
            Apps = (IEnumerable<Application>)info.GetValue("Apps", typeof(IEnumerable<Application>));

        }

        #endregion

        public class Application
        {
            public string Name { get; private set; }
            public IEnumerable<Type> Types { get; private set; }

            public Application(string type, IEnumerable<Type> types)
            {
                Name = type;
                Types = types;
            }

            #region Serialization

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Name", Name);
                info.AddValue("Types", Types);
            }

            public Application(SerializationInfo info, StreamingContext context)
            {
                Name  = (string)info.GetValue("Name", typeof(string));
                Types = (IEnumerable<Type>)info.GetValue("Types", typeof(IEnumerable<Type>));
            }

            #endregion
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

            #region Serialization

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Name", Name);
                info.AddValue("Measure", Measure);
            }

            public Type(SerializationInfo info, StreamingContext context)
            {
                Name     = (string)info.GetValue("Name", typeof(string));
                Measure  = (int)info.GetValue("Measure", typeof(int));
            }

            #endregion
        }
    }
}