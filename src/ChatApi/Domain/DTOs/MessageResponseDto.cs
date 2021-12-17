using System;
namespace ChatApi.Domain.DTOs
{
    public class MessageResponseDto
    {
        public string Content { get; init; }
        public Guid ChatRoomId { get; init; }
        public string Sender { get; init; }
        public DateTime ShippingDateTime { get; init; }

        public MessageResponseDto(string content, Guid chatRoomId, string sender, DateTime shippingDateTime)
        {
            Content = content;
            ChatRoomId = chatRoomId;
            Sender = sender;
            ShippingDateTime = shippingDateTime;
        }
    }
}
