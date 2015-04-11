using System.Linq;
using RElmah.Models;
using Xunit;

namespace RElmah.Tests.Models
{
    public class ClusterTester
    {
        [Fact]
        public void CreateWithSources()
        {
            var c = Cluster.Create("c", new[] {Source.Create("a", "a")});

            Assert.Equal(1, c.Sources.Count());
            Assert.Equal(1, c.Sources.Count(a => a.SourceId == "a"));

            var source = c.GetSource("a");
            Assert.NotNull(source);
            Assert.Equal("a", source.SourceId);
        }

        [Fact]
        public void CreateWithUsers()
        {
            var c = Cluster.Create("c", new[] { User.Create("a") });

            Assert.Equal(1, c.Users.Count());
            Assert.Equal(1, c.Users.Count(a => a.Name == "a"));
        }

        [Fact]
        public void CreateWithSourcesAndUsers()
        {
            var c = Cluster.Create("c", new[] { Source.Create("a", "a") }, new[] { User.Create("a") });

            Assert.Equal(1, c.Sources.Count());
            Assert.Equal(1, c.Sources.Count(a => a.SourceId == "a"));
            Assert.Equal(1, c.Users.Count());
            Assert.Equal(1, c.Users.Count(a => a.Name == "a"));
        }
    }
}
