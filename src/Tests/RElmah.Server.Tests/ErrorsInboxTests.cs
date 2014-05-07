using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RElmah.Domain;

namespace RElmah.Server.Tests
{
    [TestClass]
    public class ErrorsInboxTests
    {
        [TestMethod]
        public async Task PostSingleError()
        {
            var inbox = new Services.ErrorsInbox();

            var newThreadScheduler = NewThreadScheduler.Default;

            var es1 =
                from e in inbox.GetErrors()
                where e.Error != null
                select e.Error.Message;

            es1.Subscribe(s => Debug.WriteLine(s));

            newThreadScheduler.Schedule(TimeSpan.FromSeconds(1), () => inbox.Post(new ErrorPayload { Error = new ErrorDetail { Message = "Foo" } }));

            Assert.AreEqual(1, await es1.Take(1).Count());
        }
    }
}
