﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly ChatContext _dbContext;

        public UserController(ChatContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] User user)
        {
            user.DbContext = _dbContext;

            var errorMessage = await user.Create();

            if (!string.IsNullOrEmpty(errorMessage))
                return BadRequest(new ErrorResponseFactory().CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    Activity.Current.Id,
                    errorMessage
                ));

            return Created(Request.Path.Value, user);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteAsync()
        {
            var user = new User { Username = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value };
            user.DbContext = _dbContext;

            var errorMessage = user.Delete();

            if (!string.IsNullOrEmpty(errorMessage))
                return NotFound(new ErrorResponseFactory().CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    Activity.Current.Id,
                    errorMessage
                ));

            return Ok();
        }
    }
}