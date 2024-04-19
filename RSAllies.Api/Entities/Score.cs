namespace RSAllies.Api.Entities
{
    public class Score
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int ScoreValue { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
