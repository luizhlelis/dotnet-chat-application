using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BotApi.Infrastructure
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IConnection _rabbitConnection;
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;

        public MessageBroker(IConfiguration configuration, IConnection rabbitConnection)
        {
            _configuration = configuration;
            _rabbitConnection = rabbitConnection;

            _channel = _rabbitConnection.CreateModel();
        }

        public void PublishInQueue(string messageBody)
        {
            var message = JsonConvert.SerializeObject(new { Message = messageBody, Sender = "chat-bot" });
            var body = Encoding.UTF8.GetBytes(message);

            _channel.QueueDeclare(
                queue: _configuration["RabbitMq:QueueName"],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var basicProps = _channel.CreateBasicProperties();

            _channel.BasicPublish(
                exchange: "",
                routingKey: _configuration["RabbitMq:QueueName"],
                basicProperties: basicProps,
                body: body);
        }
    }
}
