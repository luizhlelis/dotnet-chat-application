using Microsoft.EntityFrameworkCore;

namespace ChatApi.Infrastructure
{
    public class ChatContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed(modelBuilder);
        }

        private static void Seed(ModelBuilder modelBuilder)
        {
            var userTest = new User("test-user", "1StrongPassword*");
            modelBuilder.Entity<User>().HasData(userTest);
        }
    }
}
