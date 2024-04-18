using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Entities;

namespace RSAllies.Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<UserResponse> UserResponses { get; set; }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Score> Scores { get; set; }

        public DbSet<VenueAvailability> VenueAvailabilities { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Phone).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Venue>().HasIndex(v => v.Name).IsUnique();
        }

    }
}
