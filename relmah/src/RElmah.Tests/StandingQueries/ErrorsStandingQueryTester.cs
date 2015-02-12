using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Fakes;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.StandingQueries;
using Xunit;

namespace RElmah.Tests.StandingQueries
{
    public class ErrorsStandingQueryTester
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
            var sut = new ErrorsStandingQuery();
            var notifications = new List<NamedPayload>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("wasp")), 
                new StubINotifier 
                { 
                    ErrorStringErrorPayload = (n, p) =>
                    {
                        notifications.Add(new NamedPayload { Name = n, Payload = p });
                    }
                },
                new StubIErrorsInbox
                {
                    GetErrorsStream = () => Observable.Empty<ErrorPayload>()
                },
                new StubIDomainPersistor
                {
                    GetUserApplicationsString = _ => Task.FromResult((IEnumerable<Application>)new [] { Application.Create("a1")})
                },
                new StubIDomainPublisher());

            //Assert
            Assert.Equal(0, notifications.Count());
        }

        [Fact]
        public void OneError()
        {
            //Arrange
            var sut = new ErrorsStandingQuery();
            var notifications = new List<NamedPayload>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("wasp")),
                new StubINotifier
                {
                    ErrorStringErrorPayload = (n, p) =>
                    {
                        notifications.Add(new NamedPayload { Name = n, Payload = p });
                    }
                },
                new StubIErrorsInbox
                {
                    GetErrorsStream = () => (
                        new[]
                        {
                            new ErrorPayload("a1", new Error(), "e1", "")
                        }
                    ).ToObservable()
                },
                new StubIDomainPersistor
                {
                    GetUserApplicationsString = _ => Task.FromResult((IEnumerable<Application>)new[] { Application.Create("a1") })
                },
                new StubIDomainPublisher());

            //Assert
            Assert.Equal(1, notifications.Count());
        }

        [Fact]
        public void MultipleErrors()
        {
            //Arrange
            var sut = new ErrorsStandingQuery();
            var notifications = new List<NamedPayload>();

            //Act
            sut.Run(
                new ValueOrError<User>(User.Create("wasp")),
                new StubINotifier
                {
                    ErrorStringErrorPayload = (n, p) =>
                    {
                        notifications.Add(new NamedPayload { Name = n, Payload = p });
                    }
                },
                new StubIErrorsInbox
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
                new StubIDomainPersistor
                {
                    GetUserApplicationsString = _ => Task.FromResult((IEnumerable<Application>)new[] { Application.Create("a1") })
                },
                new StubIDomainPublisher());

            //Assert
            Assert.Equal(2, notifications.Count());
            Assert.Equal("e2", notifications.Last().Payload.ErrorId);
        }
    }
}
