using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using ChatApi.Application.Responses;
using ChatApi.Application.Settings;
using ChatApi.Domain.DTOs;
using ChatApi.Domain.Notifications;
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
        private readonly NotificationContext _notificationContext;

        public AuthController(
            TokenCredentials tokenCredentials,
            ChatContext dbContext,
            NotificationContext notificationContext)
        {
            _tokenCredentials = tokenCredentials;
            _dbContext = dbContext;
            _notificationContext = notificationContext;
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody] CredentialsDto credentials)
        {
            var user = new User(credentials.Username, credentials.Password)
            {
                TokenCredentials = _tokenCredentials,
                DbContext = _dbContext,
                NotificationContext = _notificationContext,
            };

            return Ok(user.Authenticate());
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok( new { Username = User.Identity.Name });
        }
    }
}
