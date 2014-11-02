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
            var cp = new ConfigurationHolder(new Fakes.StubIConfigurationUpdater()
            {
                AddClusterString = n => Task.FromResult(new ValueOrError<Cluster>(fc(n))),
                GetClusters = () => Task.FromResult((IEnumerable<Cluster>)cs.Values)
            });

            Delta<Cluster> pushed = null;
            cp.ObserveClusters().Subscribe(p =>
            {
                pushed = p;
            });

            //Act
            var foo = await cp.AddCluster("foo");
            var check = (await cp.GetClusters()).Single();

            //Assert
            Assert.NotNull(foo);
            Assert.True(foo.HasValue);
            Assert.NotNull(foo.Value);
            Assert.Equal("foo", foo.Value.Name);

            Assert.Equal(foo.Value.Name, check.Name);

            Assert.NotNull(pushed);
            Assert.NotNull(pushed.Target);
            Assert.Equal("foo", pushed.Target.Name);
        }
    }
}
