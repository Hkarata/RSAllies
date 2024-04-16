namespace RSAllies.Api.Contracts;

public record FilteredSessionDto
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string? VenueName { get; set; }
    public DateTime SessionDate { get; set; }
    public int VenueCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public bool IsFull { get; set; }
}