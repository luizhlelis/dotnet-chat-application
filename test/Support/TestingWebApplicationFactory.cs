using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApi.Test.Support
{
    public class TestingWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _setupAction;

        public TestingWebApplicationFactory(Action<IServiceCollection> setupAction = null)
        {
            _setupAction = setupAction ?? (services => { });
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<TStartup>()
                        .UseTestServer()
                        .ConfigureAppConfiguration(conf => conf.AddJsonFile("appsettings.json", optional: false).AddEnvironmentVariables())
                        .ConfigureTestServices(_setupAction);
                });

            return builder;
        }
    }
}
