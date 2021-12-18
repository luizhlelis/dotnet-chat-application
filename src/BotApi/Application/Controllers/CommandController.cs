using System.Threading.Tasks;
using BotApi.Domain.DTOs;
using BotApi.Domain.Entities;
using BotApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BotApi.Application.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageBroker _messageBroker;

        public CommandController(IConfiguration configuration, IMessageBroker messageBroker)
        {
            _configuration = configuration;
            _messageBroker = messageBroker;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommandDto commandDto)
        {
            var command = new Command(commandDto.Name, commandDto.Value, commandDto.ChatRoomId)
            {
                Configuration = _configuration,
                MessageBroker = _messageBroker
            };

            await command.ApplyAction();

            return Ok();
        }
    }
}
