using System.Threading.Tasks;
using BotApi.Domain.DTOs;
using BotApi.Domain.Entities;
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

        public CommandController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CommandDto commandDto)
        {
            var command = new Command(commandDto.Name, commandDto.Value)
            {
                Configuration = _configuration
            };

            await command.ApplyAction();

            return Ok();
        }
    }
}
