using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Domain.DTOs;
using ChatApi.Domain.Entities;
using ChatApi.Test.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace ChatApi.Test.Integration
{
    public class MessageControllerTest : DatabaseFixture
    {
        public MessageControllerTest() : base(TestServiceCollections.DefaultUserAuth)
        {
        }

        [Fact(DisplayName = "Should return created when sending a message")]
        public async Task ShouldReturnCreatedWhenPostMessage()
        {
            // Arrange
            var message = new MessageDto("nec ullamcorper sit amet risus nullam eget felis eget nunc", Guid.NewGuid());
            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var expected = new Message(message.Content, message.ChatRoomId, "default-user");

            // Act
            var response = await Client.PostAsync("v1/message", content);

            // Assert
            response.Should().Be201Created();

            DbContext.Messages.First()
                .Should()
                .BeEquivalentTo(
                    expected,
                    options => options.Excluding(source => source.DateTime),
                    options => options.Excluding(source => source.Id)
                );
        }

        [Theory(DisplayName = "Should return bad request when sending a message with syntax error (empty/big message)")]
        [InlineData("")]
        [InlineData("in metus vulputate eu scelerisque felis imperdiet proin fermentum leo vel orci porta non pulvinar neque laoreet suspendisse interdum consectetur libero id faucibus nisl tincidunt eget nullam non nisi est sit amet facilisis magna etiam tempor orci eu lobortis elementum nibh tellus molestie nunc non blandit massa enim nec dui")]
        public async Task ShouldReturnBadRequestWhenBreakContractPost(string message)
        {
            // Arrange
            var requestBody = new { ChatRoomId = Guid.NewGuid() , Message = message };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/message", content);

            // Assert
            response.Should().Be400BadRequest();
        }
    }
}
