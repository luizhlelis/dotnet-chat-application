using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Domain.Entities;
using ChatApi.Test.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace ChatApi.Test.Integration
{
    public class ChatRoomControllerTest : DatabaseFixture
    {
        public ChatRoomControllerTest() : base(TestServiceCollections.DefaultUserAuth)
        {
        }

        [Fact(DisplayName = "Should return created when chatroom doesn't exist")]
        public async Task ShouldReturnCreatedWhenChatRoomDoesNotExist()
        {
            // Arrange
            var roomToCreate = new ChatRoom("room-to-create");
            var content = new StringContent(JsonConvert.SerializeObject(roomToCreate), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/chat-room", content);

            // Assert
            response.Should().Be201Created();

            DbContext.ChatRoom.First(room => room.Name == roomToCreate.Name)
                .Should()
                .BeEquivalentTo(
                    roomToCreate,
                    options => options.Excluding(source => source.Id)
                );
        }

        [Fact(DisplayName = "Should return bad request when chatroom already exists")]
        public async Task ShouldReturnBadRequestWhenChatRoomExists()
        {
            // Arrange
            var testRoom = new ChatRoom("room-to-create");
            DbContext.ChatRoom.Add(testRoom);
            DbContext.SaveChanges();

            var content = new StringContent(JsonConvert.SerializeObject(testRoom), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/chat-room", content);

            // Assert
            response.Should().Be400BadRequest();
        }

        [Fact(DisplayName = "Should return ok when chatroom exists and was removed from database")]
        public async Task ShouldReturnOkWhenUserExistsAndRemove()
        {
            // Arrange
            var testRoom = new ChatRoom("room-to-delete");
            DbContext.ChatRoom.Add(testRoom);
            DbContext.SaveChanges();

            DbContext.Entry(testRoom).State = EntityState.Detached;

            // Act
            var response = await Client.DeleteAsync("v1/chat-room");

            // Assert
            response.Should().Be200Ok();
            DbContext.ChatRoom.Where(room => room.Name == testRoom.Name)
                .Should().BeNullOrEmpty();
        }

        [Theory(DisplayName = "Should return bad request when creating a chatroom with name syntax error (empty/big name)")]
        [InlineData("")]
        [InlineData("chat-name-chat-name-chat-name-chat-name-chat-name")]
        public async Task ShouldReturnBadRequestWhenBreakContractPost(string chatRoomName)
        {
            // Arrange
            var requestBody = new { Name = chatRoomName };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/chat-room", content);

            // Assert
            response.Should().Be400BadRequest();
        }

        [Theory(DisplayName = "Should return bad request when deleting a chatroom with invalid id format")]
        [InlineData("")]
        [InlineData("invalid-format")]
        public async Task ShouldReturnBadRequestWhenInvalidIdDelete(string chatRoomId)
        {
            // Act
            var response = await Client.DeleteAsync($"v1/chat-room?{chatRoomId}");

            // Assert
            response.Should().Be400BadRequest();
        }
    }
}
