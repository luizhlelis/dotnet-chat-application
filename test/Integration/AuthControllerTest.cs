using System;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;

namespace ChatApi.Test.Integration
{
    public class AuthControllerTest
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public AuthControllerTest()
        {
            // constructs the testing server with the HostBuilder configuration
            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
        }

        [Fact(DisplayName = "Should return an access token when user exists and the password matches")]
        public async Task ShouldReturnTokenWhenUserExists()
        {
            // Arrange
            var requestBody = new { Username = "test-user", Password = "1StrongPassword*" };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var expectedResponse = new Authentication {
                ExpiresIn = DateTime.Now.AddDays(1).ToString(),
                Scope = "read:chatroom update:chatroom",
                TokenType = "Bearer" };

            // Act
            var response = await _client.PostAsync("v1/auth/token", content);
            var responseMessage = await response.Content.ReadAsStringAsync();
            var authenticationResponse = JsonConvert.DeserializeObject<Authentication>(responseMessage);

            // Assert
            response.Should().Be200Ok();

            authenticationResponse
                .Should()
                .BeEquivalentTo(expectedResponse, options => options
                    .Excluding(source => source.AccessToken)
                    .Excluding(source => source.ExpiresIn)
                );
        }

        [Fact(DisplayName = "Should return unauthorized when password doesn't match")]
        public async Task ShouldReturnUnauthorizedWhenInvalidPassword()
        {
            // Arrange
            var requestBody = new { Username = "test-user", Password = "1InvalidPassword*" };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("v1/auth/token", content);

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
            var response = await _client.PostAsync("v1/auth/token", content);

            // Assert
            response
                .Should()
                .BeAs(new ErrorResponse { Error = "access_denied", ErrorDescription = "Unauthorized" });
        }
    }
}
