using System;
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
            Delta<Cluster> pushed = null;
            var cp = new ConfigurationHolder();

            cp.ObserveClusters().Subscribe(p =>
            {
                pushed = p;
            });

            //Act
            var foo = await cp.AddCluster("foo");
            //var check = (await cp.GetClusters()).Single();

            //Assert
            Assert.NotNull(foo);
            Assert.True(foo.HasValue);
            Assert.NotNull(foo.Value);
            Assert.Equal("foo", foo.Value.Name);

            //Assert.Equal(foo.Value.Name, check.Name);

            Assert.NotNull(pushed);
            Assert.NotNull(pushed.Target);
            Assert.Equal("foo", pushed.Target.Name);
        }
    }
}
