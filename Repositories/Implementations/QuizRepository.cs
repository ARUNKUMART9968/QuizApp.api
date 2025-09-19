using Microsoft.EntityFrameworkCore;
using QuizApp.Api.Data;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories.Interfaces;

namespace QuizApp.Api.Repositories.Implementations
{
    public class QuizRepository : IQuizRepository
    {
        private readonly AppDbContext _context;

        public QuizRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            return await _context.Quizzes
                .Include(q => q.Creator)
                .Include(q => q.Questions)
                .Where(q => q.IsActive)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByCreatorAsync(int creatorId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .Where(q => q.CreatedBy == creatorId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<Quiz> GetByIdAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.Creator)
                .FirstOrDefaultAsync(q => q.QuizId == id);
        }

        public async Task<Quiz> GetWithQuestionsAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.Creator)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizId == id);
        }

        public async Task<Quiz> CreateAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<Quiz> UpdateAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await GetByIdAsync(id);
            if (quiz == null) return false;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Quizzes.AnyAsync(q => q.QuizId == id);
        }
    }
}