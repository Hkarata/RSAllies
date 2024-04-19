namespace RSAllies.Api.Contracts;

public record ScoreDto
{
    public Guid UserId { get; set; }
    public int ScoreValue { get; set; }
}