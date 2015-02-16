using System;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Services;
using RElmah.Services.Inbox;
using Xunit;

namespace RElmah.Tests.Services
{
    public class ErrorsInboxTester
    {
        [Fact]
        public void IsTestingEnvironmentOk()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            ErrorPayload pushed = null;
            var inbox = new SerializedErrorsInbox();

            inbox.GetErrorsStream().Subscribe(p =>
            {
                pushed = p;
            });

            //Act
            await inbox.Post(new ErrorPayload("sourceId", new Error(), "errorId", string.Empty));

            //Assert
            Assert.NotNull(pushed);
            Assert.Equal("errorId", pushed.ErrorId);
        }
    }
}
