using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using RElmah.Common.Extensions;

namespace RElmah.Common
{
    public class Cluster : ISerializable
    {
        private readonly IImmutableDictionary<string, Source> _sources = ImmutableDictionary<string, Source>.Empty;
        private readonly IImmutableDictionary<string, User>   _users   = ImmutableDictionary<string, User>.Empty;

        public static Cluster Create(string name)
        {
            return new Cluster(name);
        }

        public static Cluster Create(string name, IEnumerable<Source> sources)
        {
            return new Cluster(name, sources);
        }

        public static Cluster Create(string name, IEnumerable<User> users)
        {
            return new Cluster(name, users);
        }

        public static Cluster Create(string name, IEnumerable<Source> sources, IEnumerable<User> users)
        {
            return new Cluster(name, sources, users);
        }

        Cluster(string name)
        {
            Name = name;
        }

        Cluster(string name, IImmutableDictionary<string, Source> sources, IImmutableDictionary<string, User> users)
        {
            Name = name;

            _users = users;
            _sources = sources;
        }

        Cluster(string name, IEnumerable<Source> sources)
        {
            Name = name;

            var builder = ImmutableDictionary.CreateBuilder<string, Source>();
            builder.AddRange(from a in sources select new KeyValuePair<string, Source>(a.SourceId, a));
            _sources = builder.ToImmutable();
        }

        Cluster(string name, IEnumerable<User> users)
        {
            Name = name;

            var builder = ImmutableDictionary.CreateBuilder<string, User>();
            builder.AddRange(from u in users select new KeyValuePair<string, User>(u.Name, u));
            _users = builder.ToImmutable();
        }

        Cluster(string name, IEnumerable<Source> sources, IEnumerable<User> users)
        {
            Name = name;

            var ub = ImmutableDictionary.CreateBuilder<string, User>();
            ub.AddRange(from u in users select new KeyValuePair<string, User>(u.Name, u));
            _users = ub.ToImmutable();

            var ab = ImmutableDictionary.CreateBuilder<string, Source>();
            ab.AddRange(from a in sources select new KeyValuePair<string, Source>(a.SourceId, a));
            _sources = ab.ToImmutable();
        }

        public string Name { get; private set; }
        public IEnumerable<Source> Sources { get { return _sources.Values; } }
        public IEnumerable<User> Users { get { return _users.Values; } }

        public Source GetSource(string sourceId)
        {
            return _sources.Get(sourceId, null);
        }

        public User GetUser(string name)
        {
            return _users.Get(name, null);
        }

        public Cluster AddSource(Source source)
        {
            return new Cluster(Name, _sources.Add(source.SourceId, source), _users);
        }

        public Cluster RemoveSource(Source source)
        {
            return RemoveSource(source.SourceId);
        }

        public Cluster RemoveSource(string source)
        {
            return new Cluster(Name, _sources.Remove(source), _users);
        }

        public Cluster SetUser(User user)
        {
            return new Cluster(Name, _sources, _users.SetItem(user.Name, user));
        }

        public Cluster RemoveUser(User user)
        {
            return RemoveUser((string) user.Name);
        }

        public Cluster RemoveUser(string name)
        {
            return new Cluster(Name, _sources, _users.Remove(name));
        }

        public bool HasUser(string user)
        {
            return Enumerable.Contains(_users.Keys, user);
        }

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue((string) "_sources", (object) _sources);
            info.AddValue((string) "_users", (object) _users);
        }

        public Cluster(SerializationInfo info, StreamingContext context)
        {
            Name     = (string)info.GetValue("Name", typeof(string));
            _sources = (ImmutableDictionary<string, Source>)info.GetValue("_sources", typeof(ImmutableDictionary<string, Source>));
            _users   = (ImmutableDictionary<string, User>)info.GetValue("_users", typeof(ImmutableDictionary<string, User>));
        }

        #endregion
    }
}
