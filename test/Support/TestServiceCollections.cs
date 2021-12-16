using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using ChatApi.Application;
using ChatApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChatApi.Test.Support
{
    public static class TestServiceCollections
    {
        public static void UserToDeleteAuth(IServiceCollection services)
        {
            DefaultTestServices(services);

            var userToDeleteClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "user-to-delete"),
                };

            services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();

            services.AddMvc(mvcOptions =>
                {
                    mvcOptions.Filters.Add(new AllowAnonymousFilter());
                    mvcOptions.Filters.Add(new FakeUserFilter(userToDeleteClaims));
                }
            ).AddApplicationPart(typeof(Startup).Assembly); ;
        }

        public static void DefaultTestServices(IServiceCollection services)
        {
            // DB Context injection override to transaction work as expected
            services
                .RemoveAll<ChatContext>()
                .AddDbContext<ChatContext>(
                    options => options
                        .UseSqlServer(ConfigurationManager.AppSettings["ConnectionStrings:ChatContext"])
                        .LogTo(Console.WriteLine),
                    ServiceLifetime.Singleton,
                    ServiceLifetime.Singleton
                );
        }
    }
}
