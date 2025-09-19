// Controllers/QuizController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.DTOs;
using QuizApp.Api.Services;
using System.Security.Claims;

namespace QuizApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly QuizService _quizService;
        private readonly ResultService _resultService;

        public QuizController(QuizService quizService, ResultService resultService)
        {
            _quizService = quizService;
            _resultService = resultService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuizDto>>>> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(ApiResponse<IEnumerable<QuizDto>>.SuccessResponse(quizzes));
        }

        [HttpGet("my-quizzes")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuizDto>>>> GetMyQuizzes()
        {
            var userId = GetCurrentUserId();
            var quizzes = await _quizService.GetQuizzesByCreatorAsync(userId);
            return Ok(ApiResponse<IEnumerable<QuizDto>>.SuccessResponse(quizzes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuizDetailDto>>> GetQuiz(int id)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                var userId = GetCurrentUserId();

                // Check if user is admin and owner of the quiz for including correct answers
                var quiz = await _quizService.GetQuizByIdAsync(id, false);
                bool includeCorrectAnswers = userRole == "Admin";

                if (includeCorrectAnswers)
                {
                    quiz = await _quizService.GetQuizByIdAsync(id, true);
                }

                return Ok(ApiResponse<QuizDetailDto>.SuccessResponse(quiz));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<QuizDetailDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<QuizDto>>> CreateQuiz([FromBody] CreateQuizDto createQuizDto)
        {
            var userId = GetCurrentUserId();
            var quiz = await _quizService.CreateQuizAsync(createQuizDto, userId);
            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizId },
                ApiResponse<QuizDto>.SuccessResponse(quiz, "Quiz created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<QuizDto>>> UpdateQuiz(int id, [FromBody] UpdateQuizDto updateQuizDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var quiz = await _quizService.UpdateQuizAsync(id, updateQuizDto, userId);
                return Ok(ApiResponse<QuizDto>.SuccessResponse(quiz, "Quiz updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<QuizDto>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteQuiz(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _quizService.DeleteQuizAsync(id, userId);

                if (!result)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Quiz not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Quiz deleted successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("{quizId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<ResultDto>>> SubmitQuiz(int quizId, [FromBody] SubmitAnswersDto submitAnswersDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _resultService.SubmitQuizAsync(quizId, userId, submitAnswersDto);
                return Ok(ApiResponse<ResultDto>.SuccessResponse(result, "Quiz submitted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<ResultDto>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<ResultDto>.ErrorResponse(ex.Message));
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }
    }
}