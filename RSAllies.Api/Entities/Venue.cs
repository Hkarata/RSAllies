using System.ComponentModel.DataAnnotations;

namespace RSAllies.Api.Entities
{
    public class Venue
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } // For soft delete
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<VenueAvailability>? venueAvailabilities { get; set; }
    }
}
