using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatApi.Domain.Notifications;
using ChatApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class ChatRoom : IdentifiableEntity
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; private set; }

        [JsonIgnore]
        public virtual ICollection<Message> Messages { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChatContext DbContext { get; set; }

        [NotMapped]
        [JsonIgnore]
        public NotificationContext NotifyContext { get; set; }

        public ChatRoom(string name)
        {
            Name = name;
        }

        public ChatRoom(string name, Guid id)
        {
            Name = name;

            if (id == Guid.Empty)
                NotifyContext.AddNotification((int)HttpStatusCode.BadRequest, "Invalid Id Format");

            Id = id;
        }

        public async Task Create()
        {
            var roomAlreadyExists = DbContext.ChatRooms.Any(room => room.Name == Name);

            if (roomAlreadyExists)
            {
                NotifyContext.AddNotification((int)HttpStatusCode.BadRequest, "ChatRoom already exists");
                return;
            }

            await DbContext.ChatRooms.AddAsync(this);
            DbContext.SaveChanges();
        }

        public void Delete()
        {
            var roomAlreadyExists = DbContext.ChatRooms.Any(room => room.Id == Id);

            if (!roomAlreadyExists)
            {
                NotifyContext.AddNotification((int)HttpStatusCode.NotFound, "ChatRoom does not exist");
                return;
            }

            DbContext.ChatRooms.Remove(this);
            DbContext.SaveChanges();
        }
    }
}
