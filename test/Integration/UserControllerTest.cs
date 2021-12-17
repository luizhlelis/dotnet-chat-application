using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Domain;
using ChatApi.Domain.DTOs;
using ChatApi.Test.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace ChatApi.Test.Integration
{
    public class UserControllerTest : DatabaseFixture
    {
        public UserControllerTest() : base(TestServiceCollections.UserToDeleteAuth)
        {
        }

        [Fact(DisplayName = "Should return created when user doesn't exist")]
        public async Task ShouldReturnCreatedWhenUserDoesNotExist()
        {
            // Arrange
            var userToCreate = new UserDto("user-to-create", "2StrongPassword*");
            var content = new StringContent(JsonConvert.SerializeObject(userToCreate), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/user", content);

            // Assert
            response.Should().Be201Created();

            DbContext.Users.First(user => user.Username == userToCreate.Username)
                .Should().BeEquivalentTo(new { Username = "user-to-create", Password = userToCreate.Password.GetHashSha256() });
        }

        [Fact(DisplayName = "Should return bad request when user already exists")]
        public async Task ShouldReturnBadRequestWhenUserExists()
        {
            // Arrange
            var requestBody = new UserDto("test-user", "2StrongPassword*");
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/user", content);

            // Assert
            response.Should().Be400BadRequest();
        }

        [Fact(DisplayName = "Should return ok when user exists and was removed from database")]
        public async Task ShouldReturnOkWhenUserExistsAndRemove()
        {
            // Arrange
            var password = "3StrongPassword*";
            var testUser = new User("user-to-delete", password.GetHashSha256());
            DbContext.Users.Add(testUser);
            DbContext.SaveChanges();

            DbContext.Entry(testUser).State = EntityState.Detached;

            // Act
            var response = await Client.DeleteAsync("v1/user");

            // Assert
            response.Should().Be200Ok();
            DbContext.Users.Where(user => user.Username == testUser.Username)
                .Should().BeNullOrEmpty();
        }

        [Fact(DisplayName = "Should return not found when user doesn't exist")]
        public async Task ShouldReturnNotFoundWhenUserDoesNotExist()
        {
            // Act
            var response = await Client.DeleteAsync("v1/user");

            // Assert
            response.Should().Be404NotFound();
        }

        [Theory(DisplayName = "Should return bad request when break contract (empty/big username or empty password)")]
        [InlineData("", "1StrongPassword*")]
        [InlineData("test-user-test-user-test-user-test-user-test-user", "")]
        [InlineData("test-user", "")]
        [InlineData("", "")]
        public async Task ShouldReturnBadRequestWhenEmptyUsernameOrPassword(string username, string password)
        {
            // Arrange
            var requestBody = new { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("v1/user", content);

            // Assert
            response
                .Should()
                .Be400BadRequest();
        }
    }
}
