namespace QuizApp.Api.DTOs
{
    public class SubmitAnswersDto
    {
        public List<AnswerSubmissionDto> Answers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class AnswerSubmissionDto
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
    }

    public class ResultDto
    {
        public int ResultId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public decimal Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime SubmittedAt { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }

    public class QuizResultDetailDto : ResultDto
    {
        public List<AnswerDetailDto> AnswerDetails { get; set; }
    }

    public class AnswerDetailDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}