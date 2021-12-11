using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("token")]
        public IActionResult Token(User user)
        {
            IActionResult response;

            response = user.Password.Equals("1StrongPassword*") && user.Username.Equals("test-user") ?
                Ok(new Authentication
                    {
                        AccessToken = string.Empty,
                        ExpiresIn = DateTime.Now.AddDays(1).ToString(),
                        Scope = "read:chatroom update:chatroom",
                        TokenType = "Bearer"
                    }) :
                Unauthorized(new ErrorResponse { Error = "access_denied", ErrorDescription = "Unauthorized" });

            return response;
        }
    }
}
