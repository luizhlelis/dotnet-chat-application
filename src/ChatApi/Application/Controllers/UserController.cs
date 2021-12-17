using System.Security.Claims;
using System.Threading.Tasks;
using ChatApi.Domain.DTOs;
using ChatApi.Domain.Notifications;
using ChatApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Application.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly ChatContext _dbContext;
        private readonly NotificationContext _notificationContext;

        public UserController(ChatContext dbContext, NotificationContext notificationContext)
        {
            _dbContext = dbContext;
            _notificationContext = notificationContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] UserDto userDto)
        {
            var user = new User(userDto.Username, userDto.Password)
            {
                DbContext = _dbContext,
                NotificationContext = _notificationContext,
            };

            await user.Create();

            return Created(Request.Path.Value, user);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteAsync()
        {
            var user = new User(HttpContext.User.FindFirst(ClaimTypes.Name).Value, string.Empty)
            {
                DbContext = _dbContext,
                NotificationContext = _notificationContext
            };

            user.Delete();

            return Ok();
        }
    }
}
