using System.Text.Json;
using QuizApp.Api.DTOs;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories;

namespace QuizApp.Api.Services
{
    public class QuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;

        public QuizService(IQuizRepository quizRepository, IQuestionRepository questionRepository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
        }

        public async Task<IEnumerable<QuizDto>> GetAllQuizzesAsync()
        {
            var quizzes = await _quizRepository.GetAllAsync();
            return quizzes.Select(MapToQuizDto);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByCreatorAsync(int creatorId)
        {
            var quizzes = await _quizRepository.GetByCreatorAsync(creatorId);
            return quizzes.Select(MapToQuizDto);
        }

        public async Task<QuizDetailDto> GetQuizByIdAsync(int quizId, bool includeCorrectAnswers = false)
        {
            var quiz = await _quizRepository.GetWithQuestionsAsync(quizId);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            return MapToQuizDetailDto(quiz, includeCorrectAnswers);
        }

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto, int creatorId)
        {
            var quiz = new Quiz
            {
                Title = createQuizDto.Title,
                Description = createQuizDto.Description,
                Duration = createQuizDto.Duration,
                CreatedBy = creatorId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdQuiz = await _quizRepository.CreateAsync(quiz);
            return MapToQuizDto(createdQuiz);
        }

        public async Task<QuizDto> UpdateQuizAsync(int quizId, UpdateQuizDto updateQuizDto, int userId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            if (quiz.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own quizzes");
            }

            quiz.Title = updateQuizDto.Title;
            quiz.Description = updateQuizDto.Description;
            quiz.Duration = updateQuizDto.Duration;
            quiz.IsActive = updateQuizDto.IsActive;

            var updatedQuiz = await _quizRepository.UpdateAsync(quiz);
            return MapToQuizDto(updatedQuiz);
        }

        public async Task<bool> DeleteQuizAsync(int quizId, int userId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                return false;
            }

            if (quiz.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own quizzes");
            }

            return await _quizRepository.DeleteAsync(quizId);
        }

        public async Task<QuestionDto> AddQuestionAsync(int quizId, CreateQuestionDto createQuestionDto, int userId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            if (quiz.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only add questions to your own quizzes");
            }

            var question = new Question
            {
                QuizId = quizId,
                Text = createQuestionDto.Text,
                Type = createQuestionDto.Type,
                Options = createQuestionDto.Options != null ? JsonSerializer.Serialize(createQuestionDto.Options) : null,
                CorrectAnswer = createQuestionDto.CorrectAnswer,
                Order = createQuestionDto.Order
            };

            var createdQuestion = await _questionRepository.CreateAsync(question);
            return MapToQuestionDto(createdQuestion, false);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(int questionId, UpdateQuestionDto updateQuestionDto, int userId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found");
            }

            if (question.Quiz.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only update questions in your own quizzes");
            }

            question.Text = updateQuestionDto.Text;
            question.Type = updateQuestionDto.Type;
            question.Options = updateQuestionDto.Options != null ? JsonSerializer.Serialize(updateQuestionDto.Options) : null;
            question.CorrectAnswer = updateQuestionDto.CorrectAnswer;
            question.Order = updateQuestionDto.Order;

            var updatedQuestion = await _questionRepository.UpdateAsync(question);
            return MapToQuestionDto(updatedQuestion, true);
        }

        public async Task<bool> DeleteQuestionAsync(int questionId, int userId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                return false;
            }

            if (question.Quiz.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only delete questions from your own quizzes");
            }

            return await _questionRepository.DeleteAsync(questionId);
        }

        private static QuizDto MapToQuizDto(Quiz quiz)
        {
            return new QuizDto
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                Duration = quiz.Duration,
                CreatorName = quiz.Creator?.Name,
                CreatedAt = quiz.CreatedAt,
                IsActive = quiz.IsActive,
                QuestionCount = quiz.Questions?.Count ?? 0
            };
        }

        private static QuizDetailDto MapToQuizDetailDto(Quiz quiz, bool includeCorrectAnswers)
        {
            return new QuizDetailDto
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                Duration = quiz.Duration,
                CreatorName = quiz.Creator?.Name,
                CreatedAt = quiz.CreatedAt,
                IsActive = quiz.IsActive,
                QuestionCount = quiz.Questions?.Count ?? 0,
                Questions = quiz.Questions?.Select(q => MapToQuestionDto(q, includeCorrectAnswers)).ToList() ?? new List<QuestionDto>()
            };
        }

        private static QuestionDto MapToQuestionDto(Question question, bool includeCorrectAnswer)
        {
            var questionDto = new QuestionDto
            {
                QuestionId = question.QuestionId,
                QuizId = question.QuizId,
                Text = question.Text,
                Type = question.Type,
                Options = !string.IsNullOrEmpty(question.Options) ? JsonSerializer.Deserialize<List<string>>(question.Options) : new List<string>(),
                Order = question.Order
            };

            if (includeCorrectAnswer)
            {
                return new QuestionWithAnswerDto
                {
                    QuestionId = questionDto.QuestionId,
                    QuizId = questionDto.QuizId,
                    Text = questionDto.Text,
                    Type = questionDto.Type,
                    Options = questionDto.Options,
                    Order = questionDto.Order,
                    CorrectAnswer = question.CorrectAnswer
                };
            }

            return questionDto;
        }
    }
}