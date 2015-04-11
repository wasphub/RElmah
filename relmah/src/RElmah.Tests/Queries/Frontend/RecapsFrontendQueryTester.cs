using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using RElmah.Common;
using RElmah.Domain.Fakes;
using RElmah.Errors.Fakes;
using RElmah.Foundation;
using RElmah.Notifiers.Fakes;
using RElmah.Publishers.Fakes;
using RElmah.Queries;
using RElmah.Queries.Frontend;
using Xunit;

namespace RElmah.Tests.Queries.Frontend
{
    public class RecapsFrontendQueryTester
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
            var sut = new RecapsQuery();
            var notifications = new List<NamedRecap>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("u1")), new RunTargets
                {
                    FrontendNotifier = new StubIFrontendNotifier
                    {
                        RecapStringRecap = (n, r) =>
                        {
                            notifications.Add(new NamedRecap { Name = n, Recap = r });
                        }
                    },
                    ErrorsInbox = new StubIErrorsInbox
                    {
                        GetErrorsStream = () => Observable.Empty<ErrorPayload>()
                    },
                    ErrorsBacklog = new StubIErrorsBacklog
                    {
                        GetSourcesRecapIEnumerableOfSourceFuncOfIEnumerableOfErrorPayloadInt32 = (apps, _) => Task.FromResult(new ValueOrError<Recap>(new Recap(DateTime.UtcNow, Enumerable.Empty<Recap.Source>())))
                    },
                    DomainPersistor = new StubIDomainPersistor
                    {
                        GetUserSourcesString = _ => Task.FromResult((IEnumerable<Source>)new[] { Source.Create("a1", "a1") })
                    },
                    DomainPublisher = new StubIDomainPublisher
                    {
                        GetClusterSourcesSequence = () => Observable.Empty<Delta<Relationship<Cluster, Source>>>(),
                        GetClusterUsersSequence = () => Observable.Empty<Delta<Relationship<Cluster, User>>>()
                    }
                });

            //Assert
            Assert.Equal(1, notifications.Count());
        }

        [Fact]
        public void OneDeltaEachSourcePlusStartupRecap()
        {
            var scheduler     = new TestScheduler();
                              
            //Arrange         
            var sut           = new RecapsQuery();
            var notifications = new List<NamedRecap>();

            //Act
            var user          = User.Create("u1");
            var source        = Source.Create("a1", "a1");
            var cluster       = Cluster.Create("c1", new[] {user});

            var rca = Relationship.Create(cluster, source);
            var rcu = Relationship.Create(cluster, user);

            var clusterSourcesStream = scheduler.CreateColdObservable(
                Notification.CreateOnNext(Delta.Create(rca, DeltaType.Added)).RecordAt(1)    
            );

            var clusterUsersStream = scheduler.CreateColdObservable(
                Notification.CreateOnNext(Delta.Create(rcu, DeltaType.Added)).RecordAt(2)    
            );

            var errorsStream = scheduler.CreateColdObservable(
                Notification.CreateOnNext(new ErrorPayload(source.SourceId, new Error(), "e1", "")).RecordAt(5)
            );

            sut.Run(
                new ValueOrError<User>(user), new RunTargets
                {
                    FrontendNotifier = new StubIFrontendNotifier
                    {
                        RecapStringRecap = (n, r) =>
                        {
                            notifications.Add(new NamedRecap { Name = n, Recap = r });
                        }
                    },
                    ErrorsInbox = new StubIErrorsInbox
                    {
                        GetErrorsStream = () => errorsStream
                    },
                    ErrorsBacklog = new StubIErrorsBacklog
                    {
                        GetSourcesRecapIEnumerableOfSourceFuncOfIEnumerableOfErrorPayloadInt32 = (apps, _) => Task.FromResult(
                            new ValueOrError<Recap>(
                                new Recap(
                                    DateTime.UtcNow,
                                    Enumerable.Empty<Recap.Source>())))
                    },
                    DomainPersistor = new StubIDomainPersistor
                    {
                        GetUserSourcesString = _ => Task.FromResult((IEnumerable<Source>)new[] { source })
                    },
                    DomainPublisher = new StubIDomainPublisher
                    {
                        GetClusterSourcesSequence = () => clusterSourcesStream,
                        GetClusterUsersSequence = () => clusterUsersStream
                    }
                });


            //Asserts
            
            Assert.Equal(1, notifications.Count()); //first recap is received during the subscription

            scheduler.AdvanceBy(1);
            Assert.Equal(2, notifications.Count()); //1 for initial recap, 1 for adding app, 1 for adding user

            scheduler.AdvanceBy(1);
            Assert.Equal(3, notifications.Count()); //1 for initial recap, 1 for adding app, 1 for adding user

            scheduler.AdvanceBy(1);
            
        }

    }
}