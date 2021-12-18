using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace ChatApi.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IConfigurationRoot BuildAppConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return configuration.AddEnvironmentVariables().Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = BuildAppConfiguration();

                    services.AddHostedService<Worker>();

                    var connection = new ConnectionFactory
                    {
                        HostName = configuration["RabbitMq:HostName"],
                        Port = configuration.GetValue<int>("RabbitMq:Port"),
                        UserName = configuration["RabbitMq:UserName"],
                        Password = configuration["RabbitMq:Password"],
                        VirtualHost = configuration["RabbitMq:VirtualHost"]
                    }.CreateConnection();

                    services.AddSingleton(connection);

                    // Database
                    services.AddDbContext<ChatContext>(options =>
                        options
                            .UseSqlServer(configuration.GetConnectionString("ChatContext"))
                            .LogTo(Console.WriteLine));

                });
    }
}
