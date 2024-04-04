namespace RSAllies.Api.Entities
{
    public class VenueAvailability
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public DateTime AvailableDate { get; set; }
        public bool IsDeleted { get; set; } // For soft delete
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Venue Venue { get; set; } = default!;
    }
}
