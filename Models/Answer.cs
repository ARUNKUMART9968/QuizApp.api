using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        [MaxLength(500)]
        public string? SelectedAnswer { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}