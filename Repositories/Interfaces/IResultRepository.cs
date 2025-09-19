using QuizApp.Api.Models;

namespace QuizApp.Api.Repositories
{
    public interface IResultRepository
    {
        Task<Result> GetByIdAsync(int id);
        Task<Result> GetByUserAndQuizAsync(int userId, int quizId);
        Task<IEnumerable<Result>> GetByQuizAsync(int quizId);
        Task<IEnumerable<Result>> GetByUserAsync(int userId);
        Task<Result> CreateAsync(Result result);
    }
}