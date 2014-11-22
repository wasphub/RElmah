using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Grounding;
using RElmah.Models;
using RElmah.Services;
using Xunit;

namespace RElmah.Tests
{
    public class ConfigurationHolderTester
    {
        const string ClusterName = "c1";

        [Fact]
        public async Task AddCluster()
        {
            //Arrange
            var cs = new Dictionary<string, Cluster>();
            var fc = new Func<string, Cluster>(n =>
            {
                cs.Add(n, Cluster.Create(n));
                return cs[n];
            });
            var sut = new ConfigurationHolder(new Fakes.StubIConfigurationStore
            {
                AddClusterString = n => Task.FromResult(new ValueOrError<Cluster>(fc(n))),
                GetClusters = () => Task.FromResult((IEnumerable<Cluster>)cs.Values)
            });

            Delta<Cluster> observed = null;
            sut.ObserveClusters().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var cluster = await sut.AddCluster(ClusterName);
            var check = (await sut.GetClusters()).Single();

            //Assert
            Assert.NotNull(cluster);
            Assert.True(cluster.HasValue);
            Assert.NotNull(cluster.Value);
            Assert.Equal(ClusterName, cluster.Value.Name);

            Assert.Equal(cluster.Value.Name, check.Name);

            Assert.NotNull(observed);
            Assert.NotNull(observed.Target);
            Assert.Equal(ClusterName, observed.Target.Name);
            Assert.Equal(DeltaType.Added, observed.Type);
        }

        [Fact]
        public async Task AddClusterThenRemoveCluster()
        {
            //Arrange
            var cs = new Dictionary<string, Cluster>();
            var adder = new Func<string, Cluster>(n =>
            {
                cs.Add(n, Cluster.Create(n));
                return cs[n];
            });
            var remover = new Func<string, bool>(cs.Remove);

            var sut = new ConfigurationHolder(new Fakes.StubIConfigurationStore
            {
                AddClusterString = n => Task.FromResult(new ValueOrError<Cluster>(adder(n))),
                GetClusters = () => Task.FromResult((IEnumerable<Cluster>)cs.Values),
                RemoveClusterString = n => Task.FromResult(new ValueOrError<bool>(remover(n)))
            });

            Delta<Cluster> observed = null;
            sut.ObserveClusters().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var cluster = await sut.AddCluster(ClusterName);
            var check = (await sut.GetClusters()).Single();

            //Assert
            Assert.NotNull(cluster);
            Assert.True(cluster.HasValue);
            Assert.NotNull(cluster.Value);
            Assert.Equal(ClusterName, cluster.Value.Name);

            Assert.Equal(cluster.Value.Name, check.Name);

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
    }
}
