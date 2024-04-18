namespace RSAllies.Api.Contracts;

public class ChoiceDto
{
    public Guid Id { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public bool IsAnswer { get; set; }
}