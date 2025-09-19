using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.DTOs;
using QuizApp.Api.Services;
using System.Security.Claims;

namespace QuizApp.Api.Controllers
{
    [ApiController]
    [Route("api/quiz/{quizId}/[controller]")]
    [Authorize(Roles = "Admin")]
    public class QuestionController : ControllerBase
    {
        private readonly QuizService _quizService;

        public QuestionController(QuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuestionDto>>> CreateQuestion(int quizId, [FromBody] CreateQuestionDto createQuestionDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var question = await _quizService.AddQuestionAsync(quizId, createQuestionDto, userId);
                return CreatedAtAction(nameof(GetQuestion), new { quizId, id = question.QuestionId },
                    ApiResponse<QuestionDto>.SuccessResponse(question, "Question created successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<QuestionDto>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionDto>>> GetQuestion(int quizId, int id)
        {
            // This would require additional service method - simplified for now
            return Ok(ApiResponse<QuestionDto>.SuccessResponse(null, "Question retrieved"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionDto>>> UpdateQuestion(int quizId, int id, [FromBody] UpdateQuestionDto updateQuestionDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var question = await _quizService.UpdateQuestionAsync(id, updateQuestionDto, userId);
                return Ok(ApiResponse<QuestionDto>.SuccessResponse(question, "Question updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<QuestionDto>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteQuestion(int quizId, int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _quizService.DeleteQuestionAsync(id, userId);

                if (!result)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Question not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Question deleted successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}