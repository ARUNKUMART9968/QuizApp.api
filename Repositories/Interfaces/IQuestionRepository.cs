using QuizApp.Api.Models;

namespace QuizApp.Api.Repositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId);
        Task<Question> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}