using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Missions;

public class TodaysMissionQuery(
        ApplicationDbContext dbContext,
        IMissionGenerator missionGenerator)
{
        public async Task<TodaysMissionDto> ExecuteAsync(Guid currentUserId, DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                MissionType mission = missionGenerator.GetMissions(currentUserId, now.Value).Single();

                int currentProgress = await dbContext.MissionProgresses
                        .Where(mp => mp.UserId == currentUserId && mp.Date.Date == now.Value.Date)
                        .Select(mp => mp.Count)
                        .FirstOrDefaultAsync();

                TodaysMissionDto dto = new()
                {
                        Description = mission.GetDescription(),
                        CurrentProgress = currentProgress,
                        MaxProgress = mission.GetMaxProgressCount(),
                };
                return dto;
        }
}
