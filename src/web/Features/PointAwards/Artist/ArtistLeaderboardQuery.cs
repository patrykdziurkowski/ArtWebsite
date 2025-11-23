using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.PointAwards.Artist;

public class ArtistLeaderboardQuery(ApplicationDbContext dbContext)
{
        public async Task<List<LeaderboardDto>> ExecuteAsync(int offset, int amount, TimeSpan timeSpan)
        {
                DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.Subtract(timeSpan);

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
