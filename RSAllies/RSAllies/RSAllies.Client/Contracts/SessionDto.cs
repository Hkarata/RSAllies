namespace RSAllies.Contracts;

public record SessionDto
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string? VenueName { get; set; }
    public DateTime SessionDate { get; set; }
    public int CurrentCapacity { get; set; }
}