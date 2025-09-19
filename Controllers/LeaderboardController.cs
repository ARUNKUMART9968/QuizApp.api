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
    public class LeaderboardController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<ApiResponse<QuizLeaderboardDto>>> GetQuizLeaderboard(int quizId, [FromQuery] int topCount = 10)
        {
            try
            {
                var leaderboard = await _leaderboardService.GetQuizLeaderboardAsync(quizId, topCount);
                return Ok(ApiResponse<QuizLeaderboardDto>.SuccessResponse(leaderboard));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<QuizLeaderboardDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("global")]
        public async Task<ActionResult<ApiResponse<GlobalLeaderboardDto>>> GetGlobalLeaderboard([FromQuery] int topCount = 10)
        {
            var leaderboard = await _leaderboardService.GetGlobalLeaderboardAsync(topCount);
            return Ok(ApiResponse<GlobalLeaderboardDto>.SuccessResponse(leaderboard));
        }

        [HttpGet("quiz/{quizId}/podium")]
        public async Task<ActionResult<ApiResponse<PodiumDto>>> GetQuizPodium(int quizId)
        {
            try
            {
                var podium = await _leaderboardService.GetQuizPodiumAsync(quizId);
                return Ok(ApiResponse<PodiumDto>.SuccessResponse(podium));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PodiumDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("user/stats")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<UserStatsDto>>> GetMyStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                var stats = await _leaderboardService.GetUserStatsAsync(userId);
                return Ok(ApiResponse<UserStatsDto>.SuccessResponse(stats));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserStatsDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("user/{userId}/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserStatsDto>>> GetUserStats(int userId)
        {
            try
            {
                var stats = await _leaderboardService.GetUserStatsAsync(userId);
                return Ok(ApiResponse<UserStatsDto>.SuccessResponse(stats));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserStatsDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("user/{userId}/rank/quiz/{quizId}")]
        public async Task<ActionResult<ApiResponse<int>>> GetUserRankInQuiz(int userId, int quizId)
        {
            var rank = await _leaderboardService.GetUserRankInQuizAsync(userId, quizId);
            return Ok(ApiResponse<int>.SuccessResponse(rank));
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}