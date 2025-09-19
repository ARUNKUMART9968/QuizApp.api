using QuizApp.Api.Models;

namespace QuizApp.Api.Repositories.Interfaces
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<Answer>> GetByUserAndQuizAsync(int userId, int quizId);
        Task<Answer> CreateAsync(Answer answer);
        Task<IEnumerable<Answer>> CreateManyAsync(IEnumerable<Answer> answers);
        Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId);
    }
}