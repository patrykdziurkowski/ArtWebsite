using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Leaderboard.Artist;
using web.Features.Leaderboard.Reviewer;

namespace web.Features.Leaderboard;

[ApiController]
[Authorize]
public class LeaderboardApiController(
        ArtistLeaderboardQuery artistLeaderboardQuery,
        ReviewerLeaderboardQuery reviewerLeaderboardQuery
) : ControllerBase
{
        const int ARTISTS_TO_LOAD = 20;
        const int REVIEWERS_TO_LOAD = 20;

        [HttpGet("/api/leaderboards/artists")]
        public async Task<IActionResult> LoadArtistLeaderboardEntries(
                [Range(1, int.MaxValue)] int? days,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                TimeSpan? timeSpan = (days is null) ? null : TimeSpan.FromDays(days.Value);
                List<LeaderboardDto> artists = await artistLeaderboardQuery.ExecuteAsync(
                        offset, ARTISTS_TO_LOAD, timeSpan);
                return Ok(artists);
        }

        [HttpGet("/api/leaderboards/reviewers")]
        public async Task<IActionResult> LoadReviewersLeaderboardEntries(
                [Range(1, int.MaxValue)] int? days,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                TimeSpan? timeSpan = (days is null) ? null : TimeSpan.FromDays(days.Value);
                List<LeaderboardDto> reviewers = await reviewerLeaderboardQuery.ExecuteAsync(
                        offset, REVIEWERS_TO_LOAD, timeSpan);
                return Ok(reviewers);
        }
}
