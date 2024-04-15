using System.ComponentModel.DataAnnotations;

namespace RSAllies.Api.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [MaxLength(20)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Email { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Phone { get; set; } = string.Empty;
        
        [MaxLength(12)]
        public string Password { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
