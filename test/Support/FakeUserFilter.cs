using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatApi.Test.Support
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        private readonly List<Claim> _claims;

        public FakeUserFilter(List<Claim> claims)
        {
            _claims = claims;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(_claims));
            await next();
        }
    }
}
