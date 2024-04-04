using System.ComponentModel.DataAnnotations;

namespace RSAllies.Api.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public DateTime BookingDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } // For soft delete
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = default!;
        public Session Session { get; set; } = default!;
    }
}
