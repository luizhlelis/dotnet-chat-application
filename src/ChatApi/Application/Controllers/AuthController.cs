using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using ChatApi.Application.Responses;
using ChatApi.Application.Settings;
using ChatApi.Domain.DTOs;
using ChatApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Application.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenCredentials _tokenCredentials;
        private readonly ChatContext _dbContext;

        public AuthController(
            TokenCredentials tokenCredentials,
            ChatContext dbContext)
        {
            _tokenCredentials = tokenCredentials;
            _dbContext = dbContext;
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody] CredentialsDto credentials)
        {
            IActionResult response;

            var user = new User(credentials.Username, credentials.Password)
            {
                TokenCredentials = _tokenCredentials,
                DbContext = _dbContext
            };

            response = user.AreCredentialsValid() ?
                Ok(user.Authenticate()) :
                Unauthorized(new ErrorResponseFactory().CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    Activity.Current.Id)
                );

            return response;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok( new { Username = User.Identity.Name });
        }
    }
}
