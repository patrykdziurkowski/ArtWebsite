using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Leaderboard.Artist;

public class ArtistLeaderboardQuery(ApplicationDbContext dbContext)
{
        public async Task<List<LeaderboardDto>> ExecuteAsync(int offset, int amount, TimeSpan? timeSpan = null)
        {
                DateTimeOffset cutoffTime = (timeSpan is not null)
                        ? DateTimeOffset.UtcNow.Subtract(timeSpan!.Value)
                        : DateTimeOffset.MinValue;

                return await dbContext.Artists
                        .Select(a => new LeaderboardDto
                        {
                                Name = a.Name,
                                PointsInThatTimeSpan = dbContext.ArtistPointAwards
                                        .Where(pa => pa.ArtistId == a.Id && pa.DateAwarded >= cutoffTime)
                                        .Sum(pa => pa.PointValue)
                        })
                        .OrderByDescending(dto => dto.PointsInThatTimeSpan)
                        .Skip(offset)
                        .Take(amount)
                        .ToListAsync();

        }
}
