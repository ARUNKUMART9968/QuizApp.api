using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.DTOs
{
    public class LeaderboardEntryDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public decimal Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int WrongAnswers => TotalQuestions - CorrectAnswers;
        public TimeSpan TimeTaken { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int Rank { get; set; }
        public string PerformanceLevel { get; set; } // Excellent, Good, Average, Poor
    }

    public class QuizLeaderboardDto
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public string QuizDescription { get; set; }
        public int TotalParticipants { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
        public List<LeaderboardEntryDto> TopPerformers { get; set; } = new List<LeaderboardEntryDto>();
    }

    public class GlobalLeaderboardDto
    {
        public int TotalStudents { get; set; }
        public int TotalQuizzes { get; set; }
        public decimal OverallAverageScore { get; set; }
        public List<StudentPerformanceDto> TopStudents { get; set; } = new List<StudentPerformanceDto>();
    }

    public class StudentPerformanceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int QuizzesAttempted { get; set; }
        public decimal AverageScore { get; set; }
        public decimal BestScore { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalWrongAnswers => TotalQuestions - TotalCorrectAnswers;
        public decimal AccuracyPercentage => TotalQuestions > 0 ? ((decimal)TotalCorrectAnswers / TotalQuestions) * 100 : 0;
        public int GlobalRank { get; set; }
        public string PerformanceLevel { get; set; }
        public DateTime LastAttempt { get; set; }
    }

    public class PodiumDto
    {
        public LeaderboardEntryDto FirstPlace { get; set; }
        public LeaderboardEntryDto SecondPlace { get; set; }
        public LeaderboardEntryDto ThirdPlace { get; set; }
        public List<LeaderboardEntryDto> TopTen { get; set; } = new List<LeaderboardEntryDto>();
    }

    public class UserStatsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TotalQuizzesAttempted { get; set; }
        public decimal AverageScore { get; set; }
        public decimal BestScore { get; set; }
        public decimal WorstScore { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int TotalWrongAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public decimal AccuracyRate { get; set; }
        public TimeSpan TotalTimeSpent { get; set; }
        public TimeSpan AverageTimePerQuiz { get; set; }
        public List<QuizPerformanceDto> RecentQuizzes { get; set; } = new List<QuizPerformanceDto>();
    }

    public class QuizPerformanceDto
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public decimal Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public DateTime AttemptedAt { get; set; }
        public int RankInQuiz { get; set; }
    }
}