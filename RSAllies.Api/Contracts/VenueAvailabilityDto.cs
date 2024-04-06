namespace RSAllies.Api.Contracts;

public record VenueAvailabilityDto
{
    public Guid VenueId { get; set; }
    public DateTime AvailableDate { get; set; }
}