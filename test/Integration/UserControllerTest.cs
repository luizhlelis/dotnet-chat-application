using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Domain;
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
            var userToCreate = new User("user-to-create", "2StrongPassword*");
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
            var requestBody = new User("test-user", "2StrongPassword*");
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

            var local = DbContext.Set<User>()
                .Local
                .FirstOrDefault(entry => entry.Username == "user-to-delete");
            DbContext.Entry(local).State = EntityState.Detached;

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
    }
}
