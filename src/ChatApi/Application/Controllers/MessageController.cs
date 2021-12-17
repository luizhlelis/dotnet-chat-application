using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApi.Domain.DTOs;
using ChatApi.Domain.Entities;
using ChatApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Application.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class MessageController : Controller
    {
        private readonly ChatContext _dbContext;

        public MessageController(ChatContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] MessageDto messageDto)
        {
            var message = new Message(messageDto.Content,
                HttpContext.User.FindFirst(ClaimTypes.Name).Value,
                messageDto.ChatRoomId)
            {
                DbContext = _dbContext
            };

            await message.Send();

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<List<Message>> Get([FromQuery] Guid chatRoomId)
        {
            var message = new Message(){ DbContext = _dbContext };

            var response = await message.GetAllAsync(message => message.ChatRoomId == chatRoomId);

            return response;
        }
    }
}
