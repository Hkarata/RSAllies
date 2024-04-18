namespace RSAllies.Api.Contracts;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<Choice>? Choices { get; set; }
}