using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Infrastructure;
using ChatApi.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApi.Controllers
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
        public IActionResult Token(User user)
        {
            IActionResult response;

            user.TokenCredentials = _tokenCredentials;
            user.DbContext = _dbContext;

            response = user.AreCredentialsValid() ?
                Ok(user.Authenticate()) :
                Unauthorized(new ErrorResponse { Error = "access_denied", ErrorDescription = "Unauthorized" });

            return response;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok( new { User.Identity.Name });
        }
    }
}
