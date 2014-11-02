using System;
using System.Threading.Tasks;
using RElmah.Models.Errors;
using Xunit;

namespace RElmah.Tests
{
    public class ErrorsInbox
    {
        [Fact]
        public void IsTestingEnvisonmentOk()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            ErrorPayload pushed = null;
            var inbox = new Services.ErrorsInbox();

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
