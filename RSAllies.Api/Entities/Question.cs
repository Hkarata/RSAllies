namespace RSAllies.Api.Entities
{
    public class Question
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public required List<Choice> Choices { get; set; }
    }
}
