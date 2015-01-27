using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Fakes;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Subscriptions;
using Xunit;

namespace RElmah.Tests.Subscriptions
{
    public class RecapsSubscriptionTester
    {
        class NamedRecap
        {
            public string Name;
            public Recap Recap;
        }

        [Fact]
        public void NoDeltasPlusStartupRecap()
        {
            //Arrange
            var sut = new RecapsSubscription();
            var notifications = new List<NamedRecap>();

            //Act
            sut.Subscribe(
                new ValueOrError<User>(User.Create("u1")),
                new StubINotifier
                {
                    RecapStringRecap = (n, r) =>
                    {
                        notifications.Add(new NamedRecap { Name = n, Recap = r });
                    }
                },
                new StubIErrorsInbox
                {
                    GetErrorsStream = () => Observable.Empty<ErrorPayload>(),
                    GetApplicationsRecapIEnumerableOfApplication = apps => Task.FromResult(new ValueOrError<Recap>(new Recap(DateTime.UtcNow, Enumerable.Empty<Recap.Application>())))
                },
                new StubIDomainPersistor
                {
                    GetUserApplicationsString = _ => Task.FromResult((IEnumerable<Application>)new[] { Application.Create("a1") })
                },
                new StubIDomainPublisher
                {
                    GetClusterApplicationsSequence = () => Observable.Empty<Delta<Relationship<Cluster, Application>>>(),
                    GetClusterUsersSequence        = () => Observable.Empty<Delta<Relationship<Cluster, User>>>()
                });

            //Assert
            Assert.Equal(1, notifications.Count());
        }

        [Fact]
        public void OneDeltaEachSourcePlusStartupRecap()
        {
            //Arrange
            var sut = new RecapsSubscription();
            var notifications = new List<NamedRecap>();

            //Act
            var user        = User.Create("u1");
            var application = Application.Create("a1");
            var cluster     = Cluster.Create("c1", new[] {user});

            sut.Subscribe(
                new ValueOrError<User>(user),
                new StubINotifier
                {
                    RecapStringRecap = (n, r) =>
                    {
                        notifications.Add(new NamedRecap { Name = n, Recap = r });
                    }
                },
                new StubIErrorsInbox
                {
                    GetErrorsStream = () => (
                        new[]
                        {
                            new ErrorPayload(application.Name, new Error(), "e1", "")
                        }
                        ).ToObservable(),
                    GetApplicationsRecapIEnumerableOfApplication = apps => Task.FromResult(
                        new ValueOrError<Recap>(
                            new Recap(
                                DateTime.UtcNow, 
                                Enumerable.Empty<Recap.Application>())))
                },
                new StubIDomainPersistor
                {
                    GetUserApplicationsString = _ => Task.FromResult((IEnumerable<Application>)new[] { application })
                },
                new StubIDomainPublisher
                {
                    GetClusterApplicationsSequence = () => (
                        new [] 
                        { 
                            new Delta<Relationship<Cluster, Application>>(
                                new Relationship<Cluster, Application>(
                                    cluster, 
                                    application), 
                                DeltaType.Added) 
                        }).ToObservable(),
                    GetClusterUsersSequence = () => (
                        new[] 
                        { 
                            new Delta<Relationship<Cluster, User>>(
                                new Relationship<Cluster, User>(
                                    cluster, 
                                    user), 
                                DeltaType.Added) 
                        }).ToObservable()
                });

            //Assert
            Assert.Equal(3, notifications.Count()); //1 for initial recap, 1 for adding app, 1 for adding user
        }

    }
}