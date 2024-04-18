namespace RSAllies.Api.Entities
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }
        public required User User { get; set; }
    }
}
