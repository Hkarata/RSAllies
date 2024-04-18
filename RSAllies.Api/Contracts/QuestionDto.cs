namespace RSAllies.Api.Contracts;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<ChoiceDto>? Choices { get; set; }
    public bool? IsEnglish { get; set; }
}