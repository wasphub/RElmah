using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RElmah.Common
{
    public class Recap : ISerializable
    {
        public IEnumerable<Source> Sources { get; private set; }
        public DateTime When { get; private set; }

        public Recap(DateTime when, IEnumerable<Source> sources)
        {
            When = when;
            Sources = sources;
        }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("When", When);
            info.AddValue("Sources", Sources);
        }

        public Recap(SerializationInfo info, StreamingContext context)
        {
            When    = (DateTime)info.GetValue("When", typeof(DateTime));
            Sources = (IEnumerable<Source>)info.GetValue("Sources", typeof(IEnumerable<Source>));
        }

        #endregion

        public class Source
        {
            public string SourceId { get; private set; }
            public IEnumerable<Type> Types { get; private set; }

            public Source(string sourceId, IEnumerable<Type> types)
            {
                SourceId  = sourceId;
                Types     = types;
            }

            #region Serialization

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("SourceId", SourceId);
                info.AddValue("Types", Types);
            }

            public Source(SerializationInfo info, StreamingContext context)
            {
                SourceId  = (string)info.GetValue("SourceId", typeof(string));
                Types     = (IEnumerable<Type>)info.GetValue("Types", typeof(IEnumerable<Type>));
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
                info.AddValue("SourceId", Name);
                info.AddValue("Measure", Measure);
            }

            public Type(SerializationInfo info, StreamingContext context)
            {
                Name     = (string)info.GetValue("SourceId", typeof(string));
                Measure  = (int)info.GetValue("Measure", typeof(int));
            }

            #endregion
        }
    }
}