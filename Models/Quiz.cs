using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public int Duration { get; set; } // Duration in minutes

        public int CreatedBy { get; set; } // Foreign key to User
        public User Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<Result> Results { get; set; } = new List<Result>(); // FIXED: was 'r'
    }
}