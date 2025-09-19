using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.DTOs
{
    public class CreateQuizDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Range(1, 300)]
        public int Duration { get; set; } // Duration in minutes
    }

    public class QuizDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int QuestionCount { get; set; }
    }

    public class QuizDetailDto : QuizDto
    {
        public List<QuestionDto> Questions { get; set; }
    }

    public class UpdateQuizDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Range(1, 300)]
        public int Duration { get; set; }

        public bool IsActive { get; set; }
    }
}