using System;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatApi.Worker
{
    public static class RabbitMqHelper
    {
        public static void StartConsumer(
            IModel rabbitChanel,
            IConfiguration configuration,
            Action<BasicDeliverEventArgs> messageHandler)
        {
            var rabbitEventConsumer = new EventingBasicConsumer(rabbitChanel);

            rabbitEventConsumer.Received += (model, eventArgs) => messageHandler(eventArgs);

            rabbitChanel.BasicConsume(
                queue: configuration["RabbitMq:QueueName"],
                autoAck: true,
                consumer: rabbitEventConsumer);
        }
    }
}
