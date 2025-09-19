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
            var userId = GetCurrentUserId();
            var hasAttempted = await _resultService.HasUserAttemptedQuizAsync(userId, quizId);
            return Ok(ApiResponse<bool>.SuccessResponse(hasAttempted));
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}