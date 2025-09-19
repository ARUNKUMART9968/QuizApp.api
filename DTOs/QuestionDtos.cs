using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.DTOs
{
    public class CreateQuestionDto
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Type { get; set; } // "MultipleChoice", "TrueFalse"

        public List<string> Options { get; set; } // For multiple choice

        [Required]
        public string CorrectAnswer { get; set; }

        public int Order { get; set; }
    }

    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public List<string> Options { get; set; }
        public int Order { get; set; }
        // Note: CorrectAnswer is not included for students
    }

    public class QuestionWithAnswerDto : QuestionDto
    {
        public string CorrectAnswer { get; set; }
    }

    public class UpdateQuestionDto
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Type { get; set; }

        public List<string> Options { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        public int Order { get; set; }
    }
}
