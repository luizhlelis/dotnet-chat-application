using System;
namespace ChatApi.Worker
{
    public class MessageFromBot
    {
        public string Content { get; set; }
        public Guid ChatRoomId { get; set; }
        public string Sender { get; set; }
    }
}
