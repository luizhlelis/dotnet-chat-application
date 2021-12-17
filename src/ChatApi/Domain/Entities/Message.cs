using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Domain.Notifications;
using ChatApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Domain.Entities
{
    [Index(nameof(ShippingDateTime))]
    [Index(nameof(ChatRoomId))]
    public class Message : IdentifiableEntity
    {
        [Required]
        [MaxLength(300)]
        public string Content { get; private set; }

        [Required]
        [MaxLength(20)]
        public string Sender { get; private set; }

        [Required]
        public DateTime ShippingDateTime { get; private set; }

        [Required]
        public Guid ChatRoomId { get; private set; }

        public virtual ChatRoom ChatRoom { get; set; }

        [NotMapped]
        public ChatContext DbContext { get; set; }

        [NotMapped]
        public NotificationContext NotifyContext { get; set; }

        public Message(string content, string sender, Guid chatRoomId)
        {
            Content = content;
            Sender = sender;
            ChatRoomId = chatRoomId;
        }

        public async Task Send()
        {
            await DbContext.Messages.AddAsync(this);
            DbContext.SaveChanges();
        }
    }
}
