namespace RSAllies.Contracts;

public record CreateBookingDto
{
    public Guid UserId { get; set; }
    public Guid SessionId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
}