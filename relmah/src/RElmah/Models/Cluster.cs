using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RElmah.Extensions;

namespace RElmah.Models
{
    public class Cluster
    {
        private readonly IImmutableDictionary<string, Application> _applications = ImmutableDictionary<string, Application>.Empty;
        private readonly IImmutableDictionary<string, User> _users = ImmutableDictionary<string, User>.Empty;

        public static Cluster Create(string name)
        {
            return new Cluster(name);
        }

        public static Cluster Create(string name, IEnumerable<Application> applications)
        {
            return new Cluster(name, applications);
        }

        public static Cluster Create(string name, IEnumerable<User> users)
        {
            return new Cluster(name, users);
        }

        public static Cluster Create(string name, IEnumerable<Application> applications, IEnumerable<User> users)
        {
            return new Cluster(name, applications, users);
        }

        Cluster(string name)
        {
            Name = name;
        }

        Cluster(string name, IImmutableDictionary<string, Application> applications, IImmutableDictionary<string, User> users)
        {
            Name = name;

            _users = users;
            _applications = applications;
        }

        Cluster(string name, IEnumerable<Application> applications)
        {
            Name = name;

            var builder = ImmutableDictionary.CreateBuilder<string, Application>();
            builder.AddRange(from a in applications select new KeyValuePair<string, Application>(a.Name, a));
            _applications = builder.ToImmutable();
        }

        Cluster(string name, IEnumerable<User> users)
        {
            Name = name;

            var builder = ImmutableDictionary.CreateBuilder<string, User>();
            builder.AddRange(from u in users select new KeyValuePair<string, User>(u.Name, u));
            _users = builder.ToImmutable();
        }

        Cluster(string name, IEnumerable<Application> applications, IEnumerable<User> users)
        {
            Name = name;

            var ub = ImmutableDictionary.CreateBuilder<string, User>();
            ub.AddRange(from u in users select new KeyValuePair<string, User>(u.Name, u));
            _users = ub.ToImmutable();

            var ab = ImmutableDictionary.CreateBuilder<string, Application>();
            ab.AddRange(from a in applications select new KeyValuePair<string, Application>(a.Name, a));
            _applications = ab.ToImmutable();
        }

        public string Name { get; private set; }
        public IEnumerable<Application> Applications { get { return _applications.Values; } }
        public IEnumerable<User> Users { get { return _users.Values; } }

        public Application GetApplication(string name)
        {
            return _applications.Get(name, null);
        }

        public User GetUser(string name)
        {
            return _users.Get(name, null);
        }

        public Cluster AddApplication(Application app)
        {
            return new Cluster(Name, _applications.Add(app.Name, app), _users);
        }

        public Cluster RemoveApplication(Application app)
        {
            return RemoveApplication(app.Name);
        }

        public Cluster RemoveApplication(string name)
        {
            return new Cluster(Name, _applications.Remove(name), _users);
        }

        public Cluster SetUser(User user)
        {
            return new Cluster(Name, _applications, _users.SetItem(user.Name, user));
        }

        public Cluster RemoveUser(User app)
        {
            return RemoveUser(app.Name);
        }

        public Cluster RemoveUser(string name)
        {
            return new Cluster(Name, _applications, _users.Remove(name));
        }

        public bool HasUser(string user)
        {
            return _users.Keys.Contains(user);
        }
    }
}
