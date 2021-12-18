using System;
namespace BotApi.Infrastructure
{
    public interface IMessageBroker
    {
        public void PublishInQueue(string messageBody, Guid chatRoomId);
    }
}
