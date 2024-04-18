namespace RSAllies.Api.Entities
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }

    }
}
