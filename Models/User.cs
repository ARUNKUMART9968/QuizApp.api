using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string Role { get; set; } // "Admin" or "Student"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}