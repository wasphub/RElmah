using System.Linq;
using RElmah.Models;
using Xunit;

namespace RElmah.Tests.Models
{
    public class ClusterTester
    {
        [Fact]
        public void CreateWithApplications()
        {
            var c = Cluster.Create("c", new[] {Application.Create("a")});

            Assert.Equal(1, c.Applications.Count());
            Assert.Equal(1, c.Applications.Count(a => a.Name == "a"));

            var application = c.GetApplication("a");
            Assert.NotNull(application);
            Assert.Equal("a", application.Name);
        }

        [Fact]
        public void CreateWithUsers()
        {
            var c = Cluster.Create("c", new[] { User.Create("a") });

            Assert.Equal(1, c.Users.Count());
            Assert.Equal(1, c.Users.Count(a => a.Name == "a"));
        }

        [Fact]
        public void CreateWithApplicationsAndUsers()
        {
            var c = Cluster.Create("c", new[] { Application.Create("a") }, new[] { User.Create("a") });

            Assert.Equal(1, c.Applications.Count());
            Assert.Equal(1, c.Applications.Count(a => a.Name == "a"));
            Assert.Equal(1, c.Users.Count());
            Assert.Equal(1, c.Users.Count(a => a.Name == "a"));
        }
    }
}
