using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApi.Domain.DTOs;
using ChatApi.Domain.Entities;
using ChatApi.Domain.Notifications;
using ChatApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Application.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ChatRoomController : Controller
    {
        private readonly ChatContext _dbContext;
        private readonly NotificationContext _notificationContext;

        public ChatRoomController(ChatContext dbContext, NotificationContext notificationContext)
        {
            _dbContext = dbContext;
            _notificationContext = notificationContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ChatRoomDto chatRoomDto)
        {
            var room = new ChatRoom(chatRoomDto.Name)
            {
                DbContext = _dbContext,
                NotifyContext = _notificationContext,
            };

            await room.Create();

            return Created(Request.Path.Value, room);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteAsync([FromQuery] Guid id)
        {
            var room = new ChatRoom(string.Empty, id)
            {
                DbContext = _dbContext,
                NotifyContext = _notificationContext
            };

            room.Delete();

            return Ok();
        }
    }
}
