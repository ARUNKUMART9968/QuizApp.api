using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Required, MaxLength(1000)]
        public string Text { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; } // "MultipleChoice", "TrueFalse"

        public string? Options { get; set; } // JSON string for multiple choice options

        [Required, MaxLength(500)]
        public string CorrectAnswer { get; set; }

        public int Order { get; set; } // Question order in quiz

        // Navigation properties
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}