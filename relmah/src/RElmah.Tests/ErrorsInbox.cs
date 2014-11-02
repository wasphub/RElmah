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

            //Act
            await inbox.Post(new ErrorPayload("foo", new Error(), "abc", string.Empty));

            //Assert
            Assert.NotNull(pushed);
        }
    }
}
