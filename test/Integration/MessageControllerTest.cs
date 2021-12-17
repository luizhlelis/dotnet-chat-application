using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Domain.DTOs;
using ChatApi.Domain.Entities;
using ChatApi.Test.Support;
using FluentAssertions;
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
            var testRoom = new ChatRoom("room-msg-test");
            DbContext.ChatRooms.Add(testRoom);
            DbContext.SaveChanges();

            var message = new MessageDto("nec ullamcorper sit amet risus nullam eget felis eget nunc", testRoom.Id);
            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var expected = new Message(message.Content, "default-user", testRoom.Id);

            // Act
            var response = await Client.PostAsync("v1/message", content);

            // Assert
            response.Should().Be201Created();

            DbContext.Messages.First()
                .Should()
                .BeEquivalentTo(expected, options =>
                    options
                        .Excluding(source => source.ShippingDateTime)
                        .Excluding(source => source.Id)
                        .Excluding(source => source.ChatRoom)
                        .Excluding(source => source.DbContext)
                );
        }

        [Fact(DisplayName = "Should return some messages")]
        public async Task ShouldReturnSomeMessages()
        {
            // Arrange
            var testRoom = new ChatRoom("room-some-msg");
            DbContext.ChatRooms.Add(testRoom);
            var message = new Message("nec ullamcorper sit amet risus nullam eget felis eget nunc", "default-user", testRoom.Id);
            var message2 = new Message("morbi tincidunt augue interdum velit euismod in pellentesque", "default-user", testRoom.Id);
            DbContext.Messages.Add(message);
            DbContext.Messages.Add(message2);
            DbContext.SaveChanges();
            var expected = new MessageResponseDto(message.Content, message.ChatRoomId, message.Sender, DateTime.UtcNow);

            // Act
            var response = await Client.GetAsync($"v1/message?chatRoomId={testRoom.Id}");
            var responseMessage = await response.Content.ReadAsStringAsync();
            var returnedMessages = JsonConvert.DeserializeObject<List<MessageResponseDto>>(responseMessage);

            // Assert
            response.Should().Be200Ok();
            returnedMessages.First().Should().BeEquivalentTo(
                expected,
                options => options.Excluding(source => source.ShippingDateTime));

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
