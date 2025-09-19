namespace QuizApp.Api.Models
{
    public class Result  // FIXED: was 'r'
    {
        public int ResultId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public decimal Score { get; set; } // Score as percentage
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan TimeTaken { get; set; }
    }
}