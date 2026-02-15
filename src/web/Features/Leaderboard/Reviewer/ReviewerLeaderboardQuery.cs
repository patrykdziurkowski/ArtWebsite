using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Leaderboard.Reviewer;

public class ReviewerLeaderboardQuery(
        ApplicationDbContext dbContext,
        IConfiguration configuration)
{
        public async Task<List<LeaderboardDto>> ExecuteAsync(int offset, int amount, TimeSpan? timeSpan = null)
        {
                DateTimeOffset cutoffTime = (timeSpan is not null)
                        ? DateTimeOffset.UtcNow.Subtract(timeSpan!.Value)
                        : DateTimeOffset.MinValue;

                return await dbContext.Reviewers
                        .Where(r => r.Name != configuration.GetValue<string>("ROOT_USERNAME"))
                        .Select(r => new LeaderboardDto
                        {
                                UserId = r.UserId,
                                Name = r.Name,
                                PointsInThatTimeSpan = dbContext.ReviewerPointAwards
                                        .Where(award => award.ReviewerId == r.Id && award.DateAwarded >= cutoffTime)
                                        .Sum(award => award.PointValue)
                        })
                        .OrderByDescending(dto => dto.PointsInThatTimeSpan)
                        .Skip(offset)
                        .Take(amount)
                        .ToListAsync();
        }
}
