using System;
using System.Net.Http;
using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;
using ChatApi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;
using ChatApi.Test.Support;

namespace ChatApi.Test.Integration
{
    public class DatabaseFixture : IDisposable
    {
        public HttpClient Client;
        private readonly TestingWebApplicationFactory<Startup> _factory;
        private readonly IDbContextTransaction _transaction;

        public DatabaseFixture()
        {
            // constructs the testing server with the HostBuilder configuration
            _factory = new TestingWebApplicationFactory<Startup>();
            Client = _factory.CreateClient();

            // Open a transaction to not commit tests changes to db
            var dbContext = _factory.Services.GetRequiredService<ChatContext>();
            _transaction = dbContext.Database.BeginTransaction();

            dbContext.Users.Add(new User("test-user", "1StrongPassword*"));
        }

        public void Dispose()
        {
            Client?.Dispose();

            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();
        }
    }

    public class AuthControllerTest : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public AuthControllerTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Should return an access token when user exists and the password matches")]
        public async Task ShouldReturnTokenWhenUserExists()
        {
            // Arrange
            var requestBody = new { Username = "test-user", Password = "1StrongPassword*" };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var expectedResponse = new Authentication { TokenType = "Bearer" };

            // Act
            var response = await _fixture.Client.PostAsync("v1/auth/token", content);
            var responseMessage = await response.Content.ReadAsStringAsync();
            var authenticationResponse = JsonConvert.DeserializeObject<Authentication>(responseMessage);

            // Assert
            response.Should().Be200Ok();

            authenticationResponse
                .Should()
                .BeEquivalentTo(expectedResponse, options => options
                    .Excluding(source => source.AccessToken)
                );
        }

        [Fact(DisplayName = "Should return username when authenticated")]
        public async Task ShouldReturnUserNameWhenAuthenticated()
        {
            // Arrange
            var requestBody = new { Username = "test-user", Password = "1StrongPassword*" };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var expectedResponse = new { Username = "test-user" };

            var authResponse = await _fixture.Client.PostAsync("v1/auth/token", content);
            var responseMessageString = await authResponse.Content.ReadAsStringAsync();
            var authResponseMessage = JsonConvert.DeserializeObject<Authentication>(responseMessageString);

            var headerAuthValue = authResponseMessage.TokenType + " " + authResponseMessage.AccessToken;

            // Act
            _fixture.Client.DefaultRequestHeaders.Add("Authorization", headerAuthValue);
            var meResponse = await _fixture.Client.GetAsync("v1/auth/me");

            // Assert
            meResponse.Should().Be200Ok().And.BeAs(expectedResponse);
        }

        [Fact(DisplayName = "Should return unauthorized when password doesn't match")]
        public async Task ShouldReturnUnauthorizedWhenInvalidPassword()
        {
            // Arrange
            var requestBody = new { Username = "test-user", Password = "1InvalidPassword*" };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await _fixture.Client.PostAsync("v1/auth/token", content);

            // Assert
            response
                .Should()
                .BeAs(new ErrorResponse { Error = "access_denied", ErrorDescription = "Unauthorized" });
        }

        [Fact(DisplayName = "Should return unauthorized when username doesn't exist")]
        public async Task ShouldReturnUnauthorizedWhenInvalidUsername()
        {
            // Arrange
            var requestBody = new { Username = "invalid-user", Password = "1StrongPassword*" };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await _fixture.Client.PostAsync("v1/auth/token", content);

            // Assert
            response
                .Should()
                .BeAs(new ErrorResponse { Error = "access_denied", ErrorDescription = "Unauthorized" });
        }
    }
}
