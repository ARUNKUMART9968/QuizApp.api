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
    public class ResultController : ControllerBase
    {
        private readonly ResultService _resultService;

        public ResultController(ResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpGet("quiz/{quizId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ResultDto>>>> GetQuizResults(int quizId)
        {
            var results = await _resultService.GetQuizResultsAsync(quizId);
            return Ok(ApiResponse<IEnumerable<ResultDto>>.SuccessResponse(results));
        }

        [HttpGet("my-results")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ResultDto>>>> GetMyResults()
        {
            var userId = GetCurrentUserId();
            var results = await _resultService.GetUserResultsAsync(userId);
            return Ok(ApiResponse<IEnumerable<ResultDto>>.SuccessResponse(results));
        }

        [HttpGet("quiz/{quizId}/my-result")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<ResultDto>>> GetMyQuizResult(int quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _resultService.GetResultAsync(userId, quizId);
                return Ok(ApiResponse<ResultDto>.SuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<ResultDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("quiz/{quizId}/my-result/detailed")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<QuizResultDetailDto>>> GetMyDetailedQuizResult(int quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _resultService.GetDetailedResultAsync(userId, quizId);
                return Ok(ApiResponse<QuizResultDetailDto>.SuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<QuizResultDetailDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("quiz/{quizId}/check-attempt")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckQuizAttempt(int quizId)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Add detailed logging to debug the issue
                Console.WriteLine($"[DEBUG] Checking attempt for User: {userId}, Quiz: {quizId}");

                var hasAttempted = await _resultService.HasUserAttemptedQuizAsync(userId, quizId);

                // IMPORTANT: Ensure we return a proper boolean value
                Console.WriteLine($"[DEBUG] Has attempted result (type: {hasAttempted.GetType().Name}): {hasAttempted}");

                // Force boolean conversion to ensure proper response
                bool result = hasAttempted == true;

                Console.WriteLine($"[DEBUG] Final boolean result: {result}");

                return Ok(ApiResponse<bool>.SuccessResponse(result, $"Attempt check completed. Has attempted: {result}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception in CheckQuizAttempt: {ex.Message}");

                // Return false on error to allow quiz attempt
                return Ok(ApiResponse<bool>.SuccessResponse(false, "Check failed, allowing attempt"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }
}