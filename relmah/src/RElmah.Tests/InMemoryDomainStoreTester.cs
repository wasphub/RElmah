using System.Linq;
using System.Threading.Tasks;
using RElmah.Grounding;
using RElmah.Models;
using RElmah.Services;
using Xunit;

namespace RElmah.Tests
{
    public class InMemoryDomainStoreTester
    {
        const string ClusterName     = "c1";
        const string ApplicationName = "a1";
        const string UserName        = "u1";

        [Fact]
        public async Task AddCluster()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddCluster(ClusterName);
            var check  = (await sut.GetClusters()).Single();
            var single = await sut.GetCluster(ClusterName);

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ClusterName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            Assert.NotNull(single);
            Assert.True(single.HasValue);
            Assert.NotNull(single.Value);
            Assert.Equal(ClusterName, single.Value.Name);
        }

        [Fact]
        public async Task AddClusterThenRemoveCluster()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddCluster(ClusterName);
            var check = (await sut.GetClusters()).SingleOrDefault( );

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ClusterName, answer.Value.Name);

            Assert.NotNull(check);
            Assert.Equal(answer.Value.Name, check.Name);

            //Act
            var r = await sut.RemoveCluster(ClusterName);
            check = (await sut.GetClusters()).SingleOrDefault();

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);
            Assert.Null(check);
        }

        [Fact]
        public async Task AddApplication()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddApplication(ApplicationName);
            var check  = (await sut.GetApplications()).Single();
            var single = await sut.GetApplication(ApplicationName);

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ApplicationName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            Assert.NotNull(single);
            Assert.True(single.HasValue);
            Assert.NotNull(single.Value);
            Assert.Equal(ApplicationName, single.Value.Name);
        }

        [Fact]
        public async Task AddApplicationThenRemoveApplication()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddApplication(ApplicationName);
            var check = (await sut.GetApplications()).SingleOrDefault();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ApplicationName, answer.Value.Name);

            Assert.NotNull(check);
            Assert.Equal(answer.Value.Name, check.Name);

            //Act
            var r = await sut.RemoveApplication(ApplicationName);
            check = (await sut.GetApplications()).SingleOrDefault();

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);
            Assert.Null(check);
        }

        [Fact]
        public async Task AddUser()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

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

            Assert.NotNull(single);
            Assert.True(single.HasValue);
            Assert.NotNull(single.Value);
            Assert.Equal(UserName, single.Value.Name);
        }

        [Fact]
        public async Task AddUserThenAddToken()
        {
            //Arrange
            var sut = new InMemoryDomainStore();
            const string token = "foo";

            //Act
            var user = await sut.AddUser(UserName);
            await sut.AddCluster(ClusterName);
            await sut.AddUserToCluster(ClusterName, UserName);

            //Assert
            Assert.NotNull(user);
            Assert.True(user.HasValue);
            Assert.NotNull(user.Value);
            Assert.Equal(UserName, user.Value.Name);

            //Act
            var ut = await sut.AddUserToken(UserName, token);

            //Assert
            Assert.NotNull(ut);
            Assert.True(user.HasValue);
            Assert.NotNull(user.Value);
            Assert.True(ut.Value.Tokens.Any(t => t == token));
            var cluster = await sut.GetCluster(ClusterName);
            Assert.True(cluster.Value.GetUser(UserName).Tokens.Any(t => t == token));
        }

        [Fact]
        public async Task AddUserThenAddTokenThenRemoveToken()
        {
            //Arrange
            var sut = new InMemoryDomainStore();
            const string token = "foo";

            //Act
            var user = await sut.AddUser(UserName);
            await sut.AddCluster(ClusterName);
            await sut.AddUserToCluster(ClusterName, UserName);

            //Assert
            Assert.NotNull(user);
            Assert.True(user.HasValue);
            Assert.NotNull(user.Value);
            Assert.Equal(UserName, user.Value.Name);

            //Act
            var withToken = await sut.AddUserToken(UserName, token);

            //Assert
            Assert.NotNull(withToken);
            Assert.True(user.HasValue);
            Assert.NotNull(user.Value);
            Assert.True(withToken.Value.Tokens.Any(t => t == token));

            //Act
            var withoutToken = await sut.RemoveUserToken(token);

            //Assert
            Assert.NotNull(withoutToken);
            Assert.True(user.HasValue);
            Assert.NotNull(user.Value);
            Assert.False(withoutToken.Value.Tokens.Any(t => t == token));
            var cluster = await sut.GetCluster(ClusterName);
            Assert.False(cluster.Value.GetUser(UserName).Tokens.Any(t => t == token));

            //Act
            var empty = await sut.RemoveUserToken("bar");

            //Assert
            Assert.False(empty.HasValue);
        }

        [Fact]
        public async Task AddUserThenRemoveUser()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddUser(UserName);
            var check = (await sut.GetUsers()).SingleOrDefault();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(UserName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            //Act
            var r = await sut.RemoveUser(UserName);
            check = (await sut.GetUsers()).SingleOrDefault();

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);
            Assert.Null(check);
        }

        [Fact]
        public async Task AddUserToCluster()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);

            //Assert
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);

            //Act
            var cuAnswer = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var cuCheck  = cuAnswer.Value.Primary.Users.Single();

            Assert.Equal(ClusterName, cuAnswer.Value.Primary.Name);
            Assert.Equal(UserName,    cuAnswer.Value.Secondary.Name);
            Assert.Equal(UserName,    cuCheck.Name);
        }

        [Fact]
        public async Task AddUserToClusterThenRemoveIt()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);

            //Assert
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);

            //Act
            var _        = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var cuAnswer = await sut.RemoveUserFromCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var cuCheck  = cuAnswer.Value.Primary.Users;

            //Assert
            Assert.Equal(ClusterName, cuAnswer.Value.Primary.Name);
            Assert.Equal(UserName, cuAnswer.Value.Secondary.Name);
            Assert.Equal(0, cuCheck.Count());
        }

        [Fact]
        public async Task AddApplicationToCluster()
        {
            //Arrange 
            var sut = new InMemoryDomainStore();

            //Act
            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
            var aAnswer = await sut.AddApplication(ApplicationName);

            //Assert
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);
            Assert.NotNull(aAnswer);
            Assert.True(aAnswer.HasValue);

            //Act
            var _         = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var caAnswer  = await sut.AddApplicationToCluster(cAnswer.Value.Name, aAnswer.Value.Name);
            var caCheck   = caAnswer.Value.Primary.Applications;

            //Assert
            Assert.Equal(ClusterName, caAnswer.Value.Primary.Name);
            Assert.Equal(ApplicationName, caAnswer.Value.Secondary.Name);
            Assert.Equal(ApplicationName, caCheck.Single().Name);
        }

        [Fact]
        public async Task AddApplicationToClusterThenRemoveIt()
        {
            //Arrange

            var sut = new InMemoryDomainStore();

            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
            var aAnswer = await sut.AddApplication(ApplicationName);
            Assert.NotNull(cAnswer);
            Assert.True(cAnswer.HasValue);
            Assert.NotNull(uAnswer);
            Assert.True(uAnswer.HasValue);
            Assert.NotNull(aAnswer);
            Assert.True(aAnswer.HasValue);


            //Act
            var _        = await sut.AddUserToCluster(cAnswer.Value.Name, uAnswer.Value.Name);
            var __       = await sut.AddApplicationToCluster(cAnswer.Value.Name, aAnswer.Value.Name);
            var caAnswer = await sut.RemoveApplicationFromCluster(cAnswer.Value.Name, aAnswer.Value.Name);
            var caCheck  = caAnswer.Value.Primary.Applications;


            //Assert
            Assert.Equal(ClusterName, caAnswer.Value.Primary.Name);
            Assert.Equal(ApplicationName, caAnswer.Value.Secondary.Name);
            Assert.Equal(0, caCheck.Count());
        }
    }
}
