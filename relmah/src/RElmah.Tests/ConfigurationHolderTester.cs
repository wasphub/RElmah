using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;
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
            var cp = new ConfigurationHolder(new Fakes.StubIConfigurationUpdater
            {
                AddClusterString = n => Task.FromResult(new ValueOrError<Cluster>(fc(n))),
                GetClusters = () => Task.FromResult((IEnumerable<Cluster>)cs.Values)
            });

            Delta<Cluster> observed = null;
            cp.ObserveClusters().Subscribe(p =>
            {
                observed = p;
            });

            //Act
            var cluster = await cp.AddCluster(ClusterName);
            var check = (await cp.GetClusters()).Single();

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
    }
}
