using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Errors.Fakes;
using RElmah.Foundation;
using RElmah.Notifiers.Fakes;
using RElmah.Publishers.Fakes;
using RElmah.Queries;
using RElmah.Queries.Frontend;
using RElmah.Visibility.Fakes;
using Xunit;

namespace RElmah.Tests.Queries.Frontend
{
    public class ErrorsFrontendQueryTester
    {
        class NamedPayload
        {
            public string Name;
            public ErrorPayload Payload;
        }

        [Fact]
        public void NoErrors()
        {
            //Arrange
            var sut = new ErrorsQuery();
            var notifications = new List<NamedPayload>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("wasp")), new RunTargets
                {
                    FrontendNotifier = new StubIFrontendNotifier
                    {
                        ErrorStringErrorPayload = (n, p) =>
                        {
                            notifications.Add(new NamedPayload { Name = n, Payload = p });
                        }
                    },
                    ErrorsInbox = new StubIErrorsInbox
                    {
                        GetErrorsStream = () => Observable.Empty<ErrorPayload>()
                    },
                    ErrorsBacklog = new StubIErrorsBacklog(),
                    VisibilityPersistor = new StubIVisibilityPersistor
                    {
                        GetUserSourcesString = _ => Task.FromResult((IEnumerable<Source>)new[] { Source.Create("a1", "a1") })
                    },
                    VisibilityPublisher = new StubIVisibilityPublisher()
                });

            //Assert
            Assert.Equal(0, notifications.Count());
        }

        [Fact]
        public void OneError()
        {
            //Arrange
            var sut = new ErrorsQuery();
            var notifications = new List<NamedPayload>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("wasp")), new RunTargets
                {
                    FrontendNotifier = new StubIFrontendNotifier
                    {
                        ErrorStringErrorPayload = (n, p) =>
                        {
                            notifications.Add(new NamedPayload { Name = n, Payload = p });
                        }
                    },
                    ErrorsInbox = new StubIErrorsInbox
                    {
                        GetErrorsStream = () => (
                            new[]
                        {
                            new ErrorPayload("a1", new Error(), "e1", "")
                        }
                        ).ToObservable()
                    },
                    ErrorsBacklog = new StubIErrorsBacklog(),
                    VisibilityPersistor = new StubIVisibilityPersistor
                    {
                        GetUserSourcesString = _ => Task.FromResult((IEnumerable<Source>)new[] { Source.Create("a1", "a1") })
                    },
                    VisibilityPublisher = new StubIVisibilityPublisher()
                });

            //Assert
            Assert.Equal(1, notifications.Count());
        }

        [Fact]
        public void MultipleErrors()
        {
            //Arrange
            var sut = new ErrorsQuery();
            var notifications = new List<NamedPayload>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("wasp")), new RunTargets
                {
                    FrontendNotifier = new StubIFrontendNotifier
                    {
                        ErrorStringErrorPayload = (n, p) =>
                        {
                            notifications.Add(new NamedPayload { Name = n, Payload = p });
                        }
                    },
                    ErrorsInbox = new StubIErrorsInbox
                    {
                        GetErrorsStream = () => (
                            new[]
                        {
                            new ErrorPayload("a1", new Error(), "e1", ""),
                            new ErrorPayload("a1", new Error(), "e2", ""),
                            new ErrorPayload("a2", new Error(), "e3", "")
                        }
                        ).ToObservable()
                    },
                    ErrorsBacklog = new StubIErrorsBacklog(),
                    VisibilityPersistor = new StubIVisibilityPersistor
                    {
                        GetUserSourcesString = _ => Task.FromResult((IEnumerable<Source>)new[] { Source.Create("a1", "a1") })
                    },
                    VisibilityPublisher = new StubIVisibilityPublisher()
                });

            //Assert
            Assert.Equal(2, notifications.Count());
            Assert.Equal("e2", notifications.Last().Payload.ErrorId);
        }
    }
}
