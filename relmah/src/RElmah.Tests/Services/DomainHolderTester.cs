using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Domain.Fakes;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Services;
using Xunit;

namespace RElmah.Tests.Services
{
    public class DomainHolderTester
    {
        const string ClusterName     = "c1";
        const string SourceName      = "a1";
        const string UserName        = "u1";

        [Fact]
        public async Task AddCluster()
        {
            //Arrange
            var store = new Dictionary<string, Cluster>();
            var adder = new Func<string, Cluster>(n =>
            {
                store.Add(n, Cluster.Create(n));
                return store[n];
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Cluster>(adder(n))),
                GetClusters      = () => Task.FromResult((IEnumerable<Cluster>)store.Values),
                GetClusterString = n => Task.FromResult(new ValueOrError<Cluster>(store[n]))
            });

            Delta<Cluster> observed = null;
            sut.GetClustersSequence().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var answer = await sut.AddCluster(ClusterName);
            var check  = (await sut.GetClusters()).Single();
            var single = await sut.GetCluster(ClusterName);

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ClusterName, answer.Value.Name);

            Assert.NotNull(single.Value);
            Assert.Equal(ClusterName, single.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Name);
            Assert.Equal(DeltaType.Added, observed.Type);
        }

        [Fact]
        public async Task AddClusterThenRemoveCluster()
        {
            //Arrange
            var store = new Dictionary<string, Cluster>();
            var adder = new Func<string, Cluster>(n =>
            {
                store.Add(n, Cluster.Create(n));
                return store[n];
            });
            var remover = new Func<string, bool>(store.Remove);

            var sut = new DomainHolder(new StubIDomainStore
            {
                AddClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Cluster>(adder(n))),
                GetClusters = () => Task.FromResult((IEnumerable<Cluster>)store.Values),
                RemoveClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<bool>(remover(n)))
            });

            Delta<Cluster> observed = null;
            sut.GetClustersSequence().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var answer = await sut.AddCluster(ClusterName);
            var check = (await sut.GetClusters()).Single();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ClusterName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Name);
            Assert.Equal(DeltaType.Added, observed.Type);

            //Act
            var r = await sut.RemoveCluster(ClusterName);

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Name);
            Assert.Equal(DeltaType.Removed, observed.Type);
        }

        [Fact]
        public async Task AddSource()
        {
            //Arrange
            var store = new Dictionary<string, Source>();
            var adder = new Func<string, Source>(n =>
            {
                store.Add(n, Source.Create(n));
                return store[n];
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddSourceStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Source>(adder(n))),
                GetSources = () => Task.FromResult((IEnumerable<Source>)store.Values),
                GetSourceString = n => Task.FromResult(new ValueOrError<Source>(store[n]))
            });

            Delta<Source> observed = null;
            sut.GetSourcesSequence().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var answer = await sut.AddSource(SourceName);
            var check = (await sut.GetSources()).Single();
            var single = await sut.GetSource(SourceName);

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(SourceName, answer.Value.SourceId);

            Assert.Equal(answer.Value.SourceId, check.SourceId);

            Assert.NotNull(single.Value);
            Assert.Equal(SourceName, single.Value.SourceId);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(SourceName, observed.Target.SourceId);
            Assert.Equal(DeltaType.Added, observed.Type);
        }

        [Fact]
        public async Task AddSourceThenRemoveSource()
        {
            //Arrange
            var store = new Dictionary<string, Source>();
            var adder = new Func<string, Source>(n =>
            {
                store.Add(n, Source.Create(n));
                return store[n];
            });
            var remover = new Func<string, bool>(store.Remove);

            var sut = new DomainHolder(new StubIDomainStore
            {
                AddSourceStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Source>(adder(n))),
                GetSources = () => Task.FromResult((IEnumerable<Source>)store.Values),
                RemoveSourceStringBoolean = (n, _) => Task.FromResult(new ValueOrError<bool>(remover(n)))
            });

            Delta<Source> observed = null;
            sut.GetSourcesSequence().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var answer = await sut.AddSource(SourceName);
            var check = (await sut.GetSources()).Single();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(SourceName, answer.Value.SourceId);

            Assert.Equal(answer.Value.SourceId, check.SourceId);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(SourceName, observed.Target.SourceId);
            Assert.Equal(DeltaType.Added, observed.Type);

            //Act
            var r = await sut.RemoveSource(SourceName);

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(SourceName, observed.Target.SourceId);
            Assert.Equal(DeltaType.Removed, observed.Type);
        }

        [Fact]
        public async Task AddUser()
        {
            //Arrange
            var store = new Dictionary<string, User>();
            var adder = new Func<string, User>(n =>
            {
                store.Add(n, User.Create(n));
                return store[n];
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<User>(adder(n))),
                GetUsers = () => Task.FromResult((IEnumerable<User>)store.Values),
                GetUserString = n => Task.FromResult(new ValueOrError<User>(store[n]))
            });

            Delta<User> observed = null;
            sut.GetUsersSequence().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var answer = await sut.AddUser(UserName);
            var check  = (await sut.GetUsers()).Single();
            var single = await sut.GetUser(UserName);

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(UserName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            Assert.NotNull(single.Value);
            Assert.Equal(UserName, single.Value.Name);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(UserName, observed.Target.Name);
            Assert.Equal(DeltaType.Added, observed.Type);
        }

        [Fact]
        public async Task AddUserThenRemoveUser()
        {
            //Arrange
            var store = new Dictionary<string, User>();
            var adder = new Func<string, User>(n =>
            {
                store.Add(n, User.Create(n));
                return store[n];
            });
            var remover = new Func<string, bool>(store.Remove);

            var sut = new DomainHolder(new StubIDomainStore
            {
                AddUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<User>(adder(n))),
                GetUsers = () => Task.FromResult((IEnumerable<User>)store.Values),
                RemoveUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<bool>(remover(n)))
            });

            Delta<User> observed = null;
            sut.GetUsersSequence().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var answer = await sut.AddUser(UserName);
            var check = (await sut.GetUsers()).Single();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(UserName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(UserName, observed.Target.Name);
            Assert.Equal(DeltaType.Added, observed.Type);

            //Act
            var r = await sut.RemoveUser(UserName);

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(UserName, observed.Target.Name);
            Assert.Equal(DeltaType.Removed, observed.Type);
        }

        [Fact]
        public async Task AddUserToCluster()
        {
            //Arrange
            var store = new
            {
                Clusters      = new Dictionary<string, Cluster>(),
                Users         = new Dictionary<string, User>(),
                ClusterUsers  = new Dictionary<string, string[]>()
            };
            var cAdder = new Func<string, Cluster>(n =>
            {
                store.Clusters.Add(n, Cluster.Create(n));
                return store.Clusters[n];
            }); 
            var uAdder = new Func<string, User>(n =>
            {
                store.Users.Add(n, User.Create(n));
                return store.Users[n];
            });
            var cuAdder = new Func<string, string, Relationship<Cluster, User>>((c, u) =>
            {
                store.ClusterUsers.Add(c, new []{ u });
                var user = store.Users[store.ClusterUsers[c][0]];
                return new Relationship<Cluster, User>(Cluster.Create(c).SetUser(user), user);
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Cluster>(cAdder(n))),
                AddUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<User>(uAdder(n))),
                AddUserToClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(cuAdder(c, u)))
            });

            Delta<Relationship<Cluster, User>> observed = null;
            sut.GetClusterUsersSequence().Subscribe(p =>
            {
                observed = p;
            });
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);


            //Act
            var cuAnswer = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var cuCheck  = cuAnswer.Value.Primary.Users.Single();


            //Assert
            Assert.Equal(ClusterName, cuAnswer.Value.Primary.Name);
            Assert.Equal(UserName,    cuAnswer.Value.Secondary.Name);
            Assert.Equal(UserName,    cuCheck.Name);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Primary.Name);
            Assert.Equal(UserName,    observed.Target.Secondary.Name);
            Assert.Equal(DeltaType.Added, observed.Type);
        }

        [Fact]
        public async Task AddUserToClusterThenRemoveIt()
        {
            //Arrange
            var store = new
            {
                Clusters = new Dictionary<string, Cluster>(),
                Users = new Dictionary<string, User>(),
                ClusterUsers = new Dictionary<string, string[]>()
            };
            var cAdder = new Func<string, Cluster>(n =>
            {
                store.Clusters.Add(n, Cluster.Create(n));
                return store.Clusters[n];
            });
            var uAdder = new Func<string, User>(n =>
            {
                store.Users.Add(n, User.Create(n));
                return store.Users[n];
            });
            var cuAdder = new Func<string, string, Relationship<Cluster, User>>((c, u) =>
            {
                store.ClusterUsers.Add(c, new[] { u });
                var user = store.Users[store.ClusterUsers[c][0]];
                return new Relationship<Cluster, User>(Cluster.Create(c).SetUser(user), user);
            });
            var cuRemover = new Func<string, string, Relationship<Cluster, User>>((c, u) =>
            {
                store.ClusterUsers[c] = new[] { u };
                var user = store.Users[u];
                return new Relationship<Cluster, User>(Cluster.Create(c), user);
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Cluster>(cAdder(n))),
                AddUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<User>(uAdder(n))),
                AddUserToClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(cuAdder(c, u))),
                RemoveUserFromClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(cuRemover(c, u)))
            });

            Delta<Relationship<Cluster, User>> observed = null;
            sut.GetClusterUsersSequence().Subscribe(p =>
            {
                observed = p;
            });
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);


            //Act
            var __       = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var cuAnswer = await sut.RemoveUserFromCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var cuCheck  = cuAnswer.Value.Primary.Users;


            //Assert
            Assert.Equal(ClusterName, cuAnswer.Value.Primary.Name);
            Assert.Equal(UserName, cuAnswer.Value.Secondary.Name);
            Assert.Equal(0, cuCheck.Count());

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Primary.Name);
            Assert.Equal(UserName, observed.Target.Secondary.Name);
            Assert.Equal(DeltaType.Removed, observed.Type);
        }

        [Fact]
        public async Task AddSourceToCluster()
        {
            //Arrange
            var store = new
            {
                Clusters = new Dictionary<string, Cluster>(),
                Users = new Dictionary<string, User>(),
                Sources = new Dictionary<string, Source>(),
                ClusterSources = new Dictionary<string, string[]>(),
                ClusterUsers = new Dictionary<string, string[]>()
            };
            var cAdder = new Func<string, Cluster>(n =>
            {
                store.Clusters.Add(n, Cluster.Create(n));
                return store.Clusters[n];
            });
            var aAdder = new Func<string, Source>(n =>
            {
                store.Sources.Add(n, Source.Create(n));
                return store.Sources[n];
            });
            var uAdder = new Func<string, User>(n =>
            {
                store.Users.Add(n, User.Create(n));
                return store.Users[n];
            });
            var cuAdder = new Func<string, string, Relationship<Cluster, User>>((c, u) =>
            {
                store.ClusterUsers.Add(c, new[] { u });
                var user = store.Users[store.ClusterUsers[c][0]];
                var cluster = store.Clusters[c].SetUser(user);
                store.Clusters[c] = cluster;
                return new Relationship<Cluster, User>(cluster, user);
            });
            var caAdder = new Func<string, string, Relationship<Cluster, Source>>((c, u) =>
            {
                store.ClusterSources.Add(c, new[] { u });
                var app = store.Sources[store.ClusterSources[c][0]];
                var cluster = store.Clusters[c].AddSource(app);
                store.Clusters[c] = cluster;
                return new Relationship<Cluster, Source>(cluster, app);
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Cluster>(cAdder(n))),
                AddUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<User>(uAdder(n))),
                AddSourceStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Source>(aAdder(n))),
                AddUserToClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(cuAdder(c, u))),
                AddSourceToClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, Source>>(caAdder(c, u)))
            });

            Delta<Relationship<Cluster, Source>> observed = null;
            sut.GetClusterSourcesSequence().Subscribe(p =>
            {
                observed = p;
            });
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
            var aAnswer = await sut.AddSource(SourceName);
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);
            Assert.NotNull(aAnswer);
            Assert.True(aAnswer.HasValue);


            //Act
            var __        = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var uabAnswer = await sut.GetUserSources(UserName);

            Assert.NotNull(uabAnswer);
            Assert.Equal(0, uabAnswer.Count());
            
            var caAnswer  = await sut.AddSourceToCluster(cAnswer.Value.Name, aAnswer.Value.SourceId);
            var uaaAnswer = await sut.GetUserSources(UserName);
            var caCheck   = caAnswer.Value.Primary.Sources;


            //Assert
            Assert.Equal(ClusterName, caAnswer.Value.Primary.Name);
            Assert.Equal(SourceName, caAnswer.Value.Secondary.SourceId);
            Assert.Equal(SourceName, caCheck.Single().SourceId);

            Assert.NotNull(uaaAnswer);
            Assert.Equal(1, uaaAnswer.Count());
            Assert.Equal(SourceName, uaaAnswer.Single().SourceId);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Primary.Name);
            Assert.Equal(SourceName, observed.Target.Secondary.SourceId);
            Assert.Equal(DeltaType.Added, observed.Type);
        }

        [Fact]
        public async Task AddSourceToClusterThenRemoveIt()
        {
            //Arrange
            var store = new
            {
                Clusters = new Dictionary<string, Cluster>(),
                Users = new Dictionary<string, User>(),
                Sources = new Dictionary<string, Source>(),
                ClusterSources = new Dictionary<string, string[]>(),
                ClusterUsers = new Dictionary<string, string[]>()
            };
            var cAdder = new Func<string, Cluster>(n =>
            {
                store.Clusters.Add(n, Cluster.Create(n));
                return store.Clusters[n];
            });
            var aAdder = new Func<string, Source>(n =>
            {
                store.Sources.Add(n, Source.Create(n));
                return store.Sources[n];
            });
            var uAdder = new Func<string, User>(n =>
            {
                store.Users.Add(n, User.Create(n));
                return store.Users[n];
            });
            var cuAdder = new Func<string, string, Relationship<Cluster, User>>((c, u) =>
            {
                store.ClusterUsers.Add(c, new[] { u });
                var user = store.Users[store.ClusterUsers[c][0]];
                var cluster = store.Clusters[c].SetUser(user);
                store.Clusters[c] = cluster;
                return new Relationship<Cluster, User>(cluster, user);
            });
            var caAdder = new Func<string, string, Relationship<Cluster, Source>>((c, u) =>
            {
                store.ClusterSources.Add(c, new[] { u });
                var app = store.Sources[store.ClusterSources[c][0]];
                var cluster = store.Clusters[c].AddSource(app);
                store.Clusters[c] = cluster;
                return new Relationship<Cluster, Source>(cluster, app);
            }); 
            var caRemover = new Func<string, string, Relationship<Cluster, Source>>((c, u) =>
            {
                store.ClusterSources[c] = new[] { u };
                var user = store.Sources[u];
                return new Relationship<Cluster, Source>(Cluster.Create(c), user);
            });
            var sut = new DomainHolder(new StubIDomainStore
            {
                AddClusterStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Cluster>(cAdder(n))),
                AddUserStringBoolean = (n, _) => Task.FromResult(new ValueOrError<User>(uAdder(n))),
                AddSourceStringBoolean = (n, _) => Task.FromResult(new ValueOrError<Source>(aAdder(n))),
                AddUserToClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(cuAdder(c, u))),
                AddSourceToClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, Source>>(caAdder(c, u))),
                RemoveSourceFromClusterStringStringBoolean = (c, u, _) => Task.FromResult(new ValueOrError<Relationship<Cluster, Source>>(caRemover(c, u)))
            });

            Delta<Relationship<Cluster, Source>> observed = null;
            sut.GetClusterSourcesSequence().Subscribe(p =>
            {
                observed = p;
            });
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
            var aAnswer = await sut.AddSource(SourceName);
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);
            Assert.NotNull(aAnswer);
            Assert.True(aAnswer.HasValue);


            //Act
            var __       = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var ___      = await sut.AddSourceToCluster(cAnswer.Value.Name, aAnswer.Value.SourceId);
            var caAnswer = await sut.RemoveSourceFromCluster(cAnswer.Value.Name, aAnswer.Value.SourceId);
            var caCheck  = caAnswer.Value.Primary.Sources;


            //Assert
            Assert.Equal(ClusterName, caAnswer.Value.Primary.Name);
            Assert.Equal(SourceName, caAnswer.Value.Secondary.SourceId);
            Assert.Equal(0, caCheck.Count());

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Primary.Name);
            Assert.Equal(SourceName, observed.Target.Secondary.SourceId);
            Assert.Equal(DeltaType.Removed, observed.Type);
        }
    }
}
