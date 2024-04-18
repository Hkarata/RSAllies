namespace RSAllies.Api.Entities
{
    public class Choice
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public Question? Question { get; set; }
    }
}
