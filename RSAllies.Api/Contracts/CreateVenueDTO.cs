namespace RSAllies.Api.Contracts;

public record CreateVenueDto
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int Capacity { get; init; }
}