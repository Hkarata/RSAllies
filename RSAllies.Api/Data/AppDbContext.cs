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

            // Seeding data for User
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            modelBuilder.Entity<User>().HasData(user);

            // Seeding data for Venue
            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = "Venue 1",
                Capacity = 100,
                Address = "123 Street, City, Country",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            modelBuilder.Entity<Venue>().HasData(venue);

            // Seeding data for VenueAvailability
            var venueAvailability = new VenueAvailability
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                AvailableDate = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            modelBuilder.Entity<VenueAvailability>().HasData(venueAvailability);

            // Seeding data for Session
            var session = new Session
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                SessionDate = DateTime.Now.AddDays(7),
                CurrentCapacity = 0,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            modelBuilder.Entity<Session>().HasData(session);

            // Seeding data for Booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SessionId = session.Id,
                BookingDate = DateTime.Now,
                Status = "Booked",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            modelBuilder.Entity<Booking>().HasData(booking);
    }

}
}
