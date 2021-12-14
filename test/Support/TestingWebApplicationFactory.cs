using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ChatApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChatApi.Test.Support
{
    public class TestingWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<TStartup>()
                        .UseTestServer()
                        .ConfigureAppConfiguration(conf => conf.AddJsonFile("appsettings.json", optional: false).AddEnvironmentVariables())
                        .ConfigureTestServices(services =>
                            // DB Context injection override to transaction work as expected
                            services
                                .RemoveAll<ChatContext>()
                                .AddDbContext<ChatContext>(
                                    options => options
                                        .UseSqlServer(ConfigurationManager.AppSettings["ConnectionStrings:ChatContext"])
                                        .LogTo(Console.WriteLine),
                                    ServiceLifetime.Singleton,
                                    ServiceLifetime.Singleton
                            )
                        );
                });

            return builder;
        }
    }
}
