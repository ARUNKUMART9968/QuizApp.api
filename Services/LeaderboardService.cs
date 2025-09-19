using Microsoft.EntityFrameworkCore;
using QuizApp.Api.Data;
using QuizApp.Api.DTOs;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories.Interfaces;

namespace QuizApp.Api.Services
{
    public class LeaderboardService
    {
        private readonly AppDbContext _context;

        public LeaderboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QuizLeaderboardDto> GetQuizLeaderboardAsync(int quizId, int topCount = 10)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
                throw new KeyNotFoundException("Quiz not found");

            var results = await _context.Results
                .Include(r => r.User)
                .Where(r => r.QuizId == quizId)
                .OrderByDescending(r => r.Score)
                .ThenBy(r => r.TimeTaken)
                .ThenBy(r => r.SubmittedAt)
                .ToListAsync();

            var leaderboardEntries = results
                .Select((result, index) => new LeaderboardEntryDto
                {
                    UserId = result.UserId,
                    UserName = result.User.Name,
                    Email = result.User.Email,
                    Score = result.Score,
                    CorrectAnswers = result.CorrectAnswers,
                    TotalQuestions = result.TotalQuestions,
                    TimeTaken = result.TimeTaken,
                    SubmittedAt = result.SubmittedAt,
                    Rank = index + 1,
                    PerformanceLevel = GetPerformanceLevel(result.Score)
                })
                .Take(topCount)
                .ToList();

            return new QuizLeaderboardDto
            {
                QuizId = quizId,
                QuizTitle = quiz.Title,
                QuizDescription = quiz.Description,
                TotalParticipants = results.Count,
                AverageScore = results.Any() ? results.Average(r => r.Score) : 0,
                HighestScore = results.Any() ? results.Max(r => r.Score) : 0,
                LowestScore = results.Any() ? results.Min(r => r.Score) : 0,
                TopPerformers = leaderboardEntries
            };
        }

        public async Task<GlobalLeaderboardDto> GetGlobalLeaderboardAsync(int topCount = 10)
        {
            var studentPerformances = await _context.Results
                .Include(r => r.User)
                .Where(r => r.User.Role == "Student")
                .GroupBy(r => r.UserId)
                .Select(g => new StudentPerformanceDto
                {
                    UserId = g.Key,
                    UserName = g.First().User.Name,
                    Email = g.First().User.Email,
                    QuizzesAttempted = g.Count(),
                    AverageScore = g.Average(r => r.Score),
                    BestScore = g.Max(r => r.Score),
                    TotalCorrectAnswers = g.Sum(r => r.CorrectAnswers),
                    TotalQuestions = g.Sum(r => r.TotalQuestions),
                    LastAttempt = g.Max(r => r.SubmittedAt)
                })
                .OrderByDescending(s => s.AverageScore)
                .ThenByDescending(s => s.BestScore)
                .ToListAsync();

            // Assign ranks and performance levels
            for (int i = 0; i < studentPerformances.Count; i++)
            {
                studentPerformances[i].GlobalRank = i + 1;
                studentPerformances[i].PerformanceLevel = GetPerformanceLevel(studentPerformances[i].AverageScore);
            }

            var totalStudents = await _context.Users.CountAsync(u => u.Role == "Student");
            var totalQuizzes = await _context.Quizzes.CountAsync();
            var overallAverage = studentPerformances.Any() ? studentPerformances.Average(s => s.AverageScore) : 0;

            return new GlobalLeaderboardDto
            {
                TotalStudents = totalStudents,
                TotalQuizzes = totalQuizzes,
                OverallAverageScore = overallAverage,
                TopStudents = studentPerformances.Take(topCount).ToList()
            };
        }

        public async Task<PodiumDto> GetQuizPodiumAsync(int quizId)
        {
            var leaderboard = await GetQuizLeaderboardAsync(quizId, 10);

            return new PodiumDto
            {
                FirstPlace = leaderboard.TopPerformers.ElementAtOrDefault(0),
                SecondPlace = leaderboard.TopPerformers.ElementAtOrDefault(1),
                ThirdPlace = leaderboard.TopPerformers.ElementAtOrDefault(2),
                TopTen = leaderboard.TopPerformers
            };
        }

        public async Task<UserStatsDto> GetUserStatsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var userResults = await _context.Results
                .Include(r => r.Quiz)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();

            if (!userResults.Any())
            {
                return new UserStatsDto
                {
                    UserId = userId,
                    UserName = user.Name,
                    TotalQuizzesAttempted = 0
                };
            }

            var recentQuizzes = new List<QuizPerformanceDto>();
            foreach (var result in userResults.Take(5))
            {
                // Get rank in this quiz
                var rank = await GetUserRankInQuizAsync(userId, result.QuizId);

                recentQuizzes.Add(new QuizPerformanceDto
                {
                    QuizId = result.QuizId,
                    QuizTitle = result.Quiz.Title,
                    Score = result.Score,
                    CorrectAnswers = result.CorrectAnswers,
                    WrongAnswers = result.TotalQuestions - result.CorrectAnswers,
                    TotalQuestions = result.TotalQuestions,
                    TimeTaken = result.TimeTaken,
                    AttemptedAt = result.SubmittedAt,
                    RankInQuiz = rank
                });
            }

            return new UserStatsDto
            {
                UserId = userId,
                UserName = user.Name,
                TotalQuizzesAttempted = userResults.Count,
                AverageScore = userResults.Average(r => r.Score),
                BestScore = userResults.Max(r => r.Score),
                WorstScore = userResults.Min(r => r.Score),
                TotalCorrectAnswers = userResults.Sum(r => r.CorrectAnswers),
                TotalWrongAnswers = userResults.Sum(r => r.TotalQuestions - r.CorrectAnswers),
                TotalQuestions = userResults.Sum(r => r.TotalQuestions),
                AccuracyRate = userResults.Sum(r => r.TotalQuestions) > 0
                    ? ((decimal)userResults.Sum(r => r.CorrectAnswers) / userResults.Sum(r => r.TotalQuestions)) * 100
                    : 0,
                TotalTimeSpent = TimeSpan.FromMilliseconds(userResults.Sum(r => r.TimeTaken.TotalMilliseconds)),
                AverageTimePerQuiz = userResults.Any()
                    ? TimeSpan.FromMilliseconds(userResults.Average(r => r.TimeTaken.TotalMilliseconds))
                    : TimeSpan.Zero,
                RecentQuizzes = recentQuizzes
            };
        }

        public async Task<int> GetUserRankInQuizAsync(int userId, int quizId)
        {
            var userResult = await _context.Results
                .FirstOrDefaultAsync(r => r.UserId == userId && r.QuizId == quizId);

            if (userResult == null)
                return 0;

            var betterResults = await _context.Results
                .CountAsync(r => r.QuizId == quizId &&
                    (r.Score > userResult.Score ||
                     (r.Score == userResult.Score && r.TimeTaken < userResult.TimeTaken) ||
                     (r.Score == userResult.Score && r.TimeTaken == userResult.TimeTaken && r.SubmittedAt < userResult.SubmittedAt)));

            return betterResults + 1;
        }

        private static string GetPerformanceLevel(decimal score)
        {
            return score switch
            {
                >= 90 => "Excellent",
                >= 75 => "Good",
                >= 60 => "Average",
                >= 40 => "Below Average",
                _ => "Poor"
            };
        }
    }
}
