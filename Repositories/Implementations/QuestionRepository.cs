using Microsoft.EntityFrameworkCore;
using QuizApp.Api.Data;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories.Interfaces;

namespace QuizApp.Api.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                .Where(q => q.QuizId == quizId)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }

        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.QuestionId == id);
        }

        public async Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await GetByIdAsync(id);
            if (question == null) return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Questions.AnyAsync(q => q.QuestionId == id);
        }
    }
}