namespace RSAllies.Client.Contracts;

public record BookingDto
{
    public Guid Id { get; set; }
    public string? VenueName { get; set; }
    public string? VenueAddress { get; set; }
    public DateTime SessionDate { get; set; }
}