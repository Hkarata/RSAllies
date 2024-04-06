namespace RSAllies.Api.Contracts;

public record CreateSessionDto
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public DateTime SessionDate { get; set; }
    public int CurrentCapacity { get; set; }
}