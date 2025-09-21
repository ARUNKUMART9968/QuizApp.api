using Microsoft.EntityFrameworkCore;
using QuizApp.Api.Data;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories.Interfaces;

namespace QuizApp.Api.Repositories.Implementations
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Answer>> GetByUserAndQuizAsync(int userId, int quizId)
        {
            return await _context.Answers
                .Include(a => a.Question)
                .Where(a => a.UserId == userId && a.QuizId == quizId)
                .ToListAsync();
        }

        public async Task<Answer> CreateAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<IEnumerable<Answer>> CreateManyAsync(IEnumerable<Answer> answers)
        {
            _context.Answers.AddRange(answers);
            await _context.SaveChangesAsync();
            return answers;
        }

        // REMOVED: This method is no longer used since we check Results table instead
        // The HasUserAttemptedQuizAsync method has been moved to ResultService
        public async Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId)
        {
            // This method is deprecated - use ResultService.HasUserAttemptedQuizAsync instead
            // Keeping for interface compatibility but logging warning
            Console.WriteLine("WARNING: AnswerRepository.HasUserAttemptedQuizAsync is deprecated. Use ResultService.HasUserAttemptedQuizAsync instead.");

            // Return false to not block students, let ResultService handle the check
            return false;
        }
    }
}