using System;
using ChatApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Infrastructure
{
    public class ChatContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Seed(modelBuilder);
        }

        private static void Seed(ModelBuilder modelBuilder)
        {
            var generalChatRoom = new ChatRoom("general", Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"));
            modelBuilder.Entity<ChatRoom>().HasData(generalChatRoom);
        }
    }
}
