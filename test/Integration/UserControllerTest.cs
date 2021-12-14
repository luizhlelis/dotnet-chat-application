using System;
using System.Threading.Tasks;
using ChatApi.Test.Support;
using Xunit;

namespace ChatApi.Test.Integration
{
    public class UserControllerTest : IClassFixture<DatabaseFixture>
    {
        public UserControllerTest()
        {
        }

        [Fact(DisplayName = "Should return created when user doesn't exist")]
        public async Task ShouldReturnCreatedWhenUserDoesNotExist()
        {
        }

        [Fact(DisplayName = "Should return bad request when user already exists")]
        public async Task ShouldReturnBadRequestWhenUserExists()
        {
        }

        [Fact(DisplayName = "Should return ok when user exists and was removed from database")]
        public async Task ShouldReturnTokenWhenUserExists()
        {
        }

        [Fact(DisplayName = "Should return not found when user doesn't exist")]
        public async Task ShouldReturnNotFoundWhenUserDoesNotExist()
        {
        }
    }
}
