using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApi.Domain.Entities;
using ChatApi.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatApi.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChanel;
        private readonly ChatContext _chatContext;

        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration,
            IConnection connection,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _rabbitConnection = connection;

            _rabbitChanel = _rabbitConnection.CreateModel();

            _chatContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ChatContext>(); ;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitChanel.QueueDeclare(
                queue: _configuration["RabbitMq:QueueName"],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            RabbitMqHelper.StartConsumer(_rabbitChanel, _configuration, MessageHandler);

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void MessageHandler(BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var messageFromBot = JsonConvert.DeserializeObject<MessageFromBot>(message);

            _chatContext.Messages.Add(new Message(
                messageFromBot.Content, messageFromBot.Sender, messageFromBot.ChatRoomId));

            _chatContext.SaveChanges();
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping queue listener...");
            _rabbitChanel.Dispose();
            _rabbitConnection.Dispose();
            await base.StopAsync(stoppingToken);
        }
    }
}
