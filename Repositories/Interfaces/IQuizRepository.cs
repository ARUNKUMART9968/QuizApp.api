using QuizApp.Api.Models;

namespace QuizApp.Api.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<IEnumerable<Quiz>> GetByCreatorAsync(int creatorId);
        Task<Quiz?> GetByIdAsync(int id);
        Task<Quiz?> GetWithQuestionsAsync(int id);
        Task<Quiz> CreateAsync(Quiz quiz);
        Task<Quiz> UpdateAsync(Quiz quiz);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}