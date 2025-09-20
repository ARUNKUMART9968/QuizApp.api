using Microsoft.EntityFrameworkCore;
using QuizApp.Api.Data;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories.Interfaces;
using 

namespace QuizApp.Api.Repositories.Implementations
{
    public class ResultRepository : IResultRepository
    {
        private readonly AppDbContext _context;

        public ResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> GetByIdAsync(int id)
        {
            return await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.ResultId == id);
        }

        public async Task<Result> GetByUserAndQuizAsync(int userId, int quizId)
        {
            return await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.QuizId == quizId);
        }

        public async Task<IEnumerable<Result>> GetByQuizAsync(int quizId)
        {
            return await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .Where(r => r.QuizId == quizId)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Result>> GetByUserAsync(int userId)
        {
            return await _context.Results
                .Include(r => r.Quiz)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<Result> CreateAsync(Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }
    }
}