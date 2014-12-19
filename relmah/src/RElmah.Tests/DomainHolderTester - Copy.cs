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

            Assert.NotNull(single.Value);
            Assert.Equal(ClusterName, single.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);
        }

        [Fact]
        public async Task AddClusterThenRemoveCluster()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddCluster(ClusterName);
            var check = (await sut.GetClusters()).Single();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ClusterName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            //Act
            var r = await sut.RemoveCluster(ClusterName);

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);
        }

        [Fact]
        public async Task AddApplication()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddApplication(ApplicationName);
            var check = (await sut.GetApplications()).Single();
            var single = await sut.GetApplication(ApplicationName);

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ApplicationName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

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
            var check = (await sut.GetApplications()).Single();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(ApplicationName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            //Act
            var r = await sut.RemoveApplication(ApplicationName);

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);
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

            Assert.NotNull(single.Value);
            Assert.Equal(UserName, single.Value.Name);
        }

        [Fact]
        public async Task AddUserThenRemoveUser()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            //Act
            var answer = await sut.AddUser(UserName);
            var check = (await sut.GetUsers()).Single();

            //Assert
            Assert.NotNull(answer);
            Assert.True(answer.HasValue);
            Assert.NotNull(answer.Value);
            Assert.Equal(UserName, answer.Value.Name);

            Assert.Equal(answer.Value.Name, check.Name);

            //Act
            var r = await sut.RemoveUser(UserName);

            //Assert
            Assert.True(r.HasValue);
            Assert.True(r.Value);
        }

        [Fact]
        public async Task AddUserToCluster()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

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
        }

        [Fact]
        public async Task AddUserToClusterThenRemoveIt()
        {
            //Arrange
            var sut = new InMemoryDomainStore();

            var cAnswer = await sut.AddCluster(ClusterName);
            var uAnswer = await sut.AddUser(UserName);
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
