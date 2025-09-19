// Services/ResultService.cs
using QuizApp.Api.DTOs;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories;
using System.Text.Json;

namespace QuizApp.Api.Services
{
    public class ResultService
    {
        private readonly IResultRepository _resultRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;

        public ResultService(
            IResultRepository resultRepository,
            IAnswerRepository answerRepository,
            IQuizRepository quizRepository,
            IQuestionRepository questionRepository)
        {
            _resultRepository = resultRepository;
            _answerRepository = answerRepository;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
        }

        public async Task<ResultDto> SubmitQuizAsync(int quizId, int userId, SubmitAnswersDto submitAnswersDto)
        {
            // Check if quiz exists
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            // Check if user has already attempted this quiz
            if (await _answerRepository.HasUserAttemptedQuizAsync(userId, quizId))
            {
                throw new ArgumentException("You have already attempted this quiz");
            }

            // Get all questions for the quiz
            var questions = await _questionRepository.GetByQuizIdAsync(quizId);
            var questionsList = questions.ToList();

            if (!questionsList.Any())
            {
                throw new ArgumentException("Quiz has no questions");
            }

            // Create answers
            var answers = new List<Answer>();
            foreach (var answerDto in submitAnswersDto.Answers)
            {
                var question = questionsList.FirstOrDefault(q => q.QuestionId == answerDto.QuestionId);
                if (question != null)
                {
                    answers.Add(new Answer
                    {
                        UserId = userId,
                        QuizId = quizId,
                        QuestionId = answerDto.QuestionId,
                        SelectedAnswer = answerDto.SelectedAnswer,
                        SubmittedAt = DateTime.UtcNow
                    });
                }
            }

            // Save answers
            await _answerRepository.CreateManyAsync(answers);

            // Calculate result
            var correctAnswers = 0;
            foreach (var answer in answers)
            {
                var question = questionsList.First(q => q.QuestionId == answer.QuestionId);
                if (string.Equals(answer.SelectedAnswer?.Trim(), question.CorrectAnswer?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    correctAnswers++;
                }
            }

            var totalQuestions = questionsList.Count;
            var score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
            var timeTaken = submitAnswersDto.EndTime - submitAnswersDto.StartTime;

            // Create result
            var result = new Result
            {
                UserId = userId,
                QuizId = quizId,
                Score = score,
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions,
                SubmittedAt = DateTime.UtcNow,
                TimeTaken = timeTaken
            };

            var createdResult = await _resultRepository.CreateAsync(result);

            return new ResultDto
            {
                ResultId = createdResult.ResultId,
                UserId = createdResult.UserId,
                UserName = "", // Will be filled when fetching with user data
                QuizId = createdResult.QuizId,
                QuizTitle = quiz.Title,
                Score = createdResult.Score,
                CorrectAnswers = createdResult.CorrectAnswers,
                TotalQuestions = createdResult.TotalQuestions,
                SubmittedAt = createdResult.SubmittedAt,
                TimeTaken = createdResult.TimeTaken
            };
        }

        public async Task<ResultDto> GetResultAsync(int userId, int quizId)
        {
            var result = await _resultRepository.GetByUserAndQuizAsync(userId, quizId);
            if (result == null)
            {
                throw new KeyNotFoundException("Result not found");
            }

            return MapToResultDto(result);
        }

        public async Task<QuizResultDetailDto> GetDetailedResultAsync(int userId, int quizId)
        {
            var result = await _resultRepository.GetByUserAndQuizAsync(userId, quizId);
            if (result == null)
            {
                throw new KeyNotFoundException("Result not found");
            }

            var answers = await _answerRepository.GetByUserAndQuizAsync(userId, quizId);
            var answersList = answers.ToList();

            var answerDetails = new List<AnswerDetailDto>();
            foreach (var answer in answersList)
            {
                var isCorrect = string.Equals(answer.SelectedAnswer?.Trim(), answer.Question.CorrectAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);

                answerDetails.Add(new AnswerDetailDto
                {
                    QuestionId = answer.QuestionId,
                    QuestionText = answer.Question.Text,
                    SelectedAnswer = answer.SelectedAnswer,
                    CorrectAnswer = answer.Question.CorrectAnswer,
                    IsCorrect = isCorrect
                });
            }

            return new QuizResultDetailDto
            {
                ResultId = result.ResultId,
                UserId = result.UserId,
                UserName = result.User.Name,
                QuizId = result.QuizId,
                QuizTitle = result.Quiz.Title,
                Score = result.Score,
                CorrectAnswers = result.CorrectAnswers,
                TotalQuestions = result.TotalQuestions,
                SubmittedAt = result.SubmittedAt,
                TimeTaken = result.TimeTaken,
                AnswerDetails = answerDetails.OrderBy(a => a.QuestionId).ToList()
            };
        }

        public async Task<IEnumerable<ResultDto>> GetQuizResultsAsync(int quizId)
        {
            var results = await _resultRepository.GetByQuizAsync(quizId);
            return results.Select(MapToResultDto);
        }

        public async Task<IEnumerable<ResultDto>> GetUserResultsAsync(int userId)
        {
            var results = await _resultRepository.GetByUserAsync(userId);
            return results.Select(MapToResultDto);
        }

        public async Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId)
        {
            return await _answerRepository.HasUserAttemptedQuizAsync(userId, quizId);
        }

        private static ResultDto MapToResultDto(Result result)
        {
            return new ResultDto
            {
                ResultId = result.ResultId,
                UserId = result.UserId,
                UserName = result.User?.Name ?? "",
                QuizId = result.QuizId,
                QuizTitle = result.Quiz?.Title ?? "",
                Score = result.Score,
                CorrectAnswers = result.CorrectAnswers,
                TotalQuestions = result.TotalQuestions,
                SubmittedAt = result.SubmittedAt,
                TimeTaken = result.TimeTaken
            };
        }
    }
}