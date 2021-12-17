using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatApi.Domain.DTOs;
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

        [Required]
        [JsonIgnore]
        public virtual ChatRoom ChatRoom { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChatContext DbContext { get; set; }

        [NotMapped]
        [JsonIgnore]
        public INotificationContext NotifyContext { get; set; }

        public Message()
        {

        }

        public Message(string content, string sender, Guid chatRoomId)
        {
            Content = content;
            Sender = sender;
            ChatRoomId = chatRoomId;
            ShippingDateTime = DateTime.UtcNow;
        }

        public async Task Send()
        {
            if (IsCommand())
                Console.WriteLine("HttpClient integration");
                // HttpClient integration

            await DbContext.Messages.AddAsync(this);
            DbContext.SaveChanges();
        }

        public async Task<List<MessageResponseDto>> GetAllAsync(Expression<Func<Message, bool>> filter)
        {
            var roomWithMessages = await DbContext.Messages
                .Where(filter)
                .Take(50)
                .OrderBy(message => message.ShippingDateTime)
                .Select(message => new MessageResponseDto(message.Content, message.ChatRoomId, message.Sender, message.ShippingDateTime))
                .ToListAsync();

            return roomWithMessages;
        }

        public bool IsCommand()
        {
            var pattern = @"^\/(.*?)\=";
            var rgx = new Regex(pattern);
            var match = rgx.Match(Content);

            if (match.Success)
            {
                AvailableCommands.Get().TryGetValue(match.Groups[1].Value, out string command);
                if (string.IsNullOrEmpty(command))
                    NotifyContext.AddNotification((int)HttpStatusCode.BadRequest, "Invalid command, try to fix the syntax");
                else
                    return true;
            }

            return false;
        }
    }
}
