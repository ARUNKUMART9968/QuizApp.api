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

        public async Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId)
        {
            Console.WriteLine($"Checking answers table for User: {userId}, Quiz: {quizId}");

            var result = await _context.Answers.AnyAsync(a => a.UserId == userId && a.QuizId == quizId);

            Console.WriteLine($"Found answers: {result}");

            return result;
        }
    }
}