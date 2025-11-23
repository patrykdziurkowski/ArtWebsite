using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.PointAwards.Reviewer;

public class ReviewerLeaderboardQuery(ApplicationDbContext dbContext)
{
        public async Task<List<LeaderboardDto>> ExecuteAsync(int offset, int amount, TimeSpan timeSpan)
        {
                DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.Subtract(timeSpan);

                return await dbContext.Reviewers
                        .Select(r => new LeaderboardDto
                        {
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
