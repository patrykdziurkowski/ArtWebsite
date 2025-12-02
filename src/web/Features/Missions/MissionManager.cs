using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.Reviewers;

namespace web.Features.Missions;

public class MissionManager(
        ApplicationDbContext dbContext,
        MissionGenerator missionGenerator)
{
        private const int POINTS_PER_QUEST = 25;

        public async Task RecordProgress(MissionType missionType, Guid userId, DateTimeOffset now)
        {
                MissionType[] missions = missionGenerator.GetMissions(userId, now);
                if (!missions.Contains(missionType))
                {
                        return;
                }

                MissionRecipient missionRecipient = Missions.GetRecipient(missionType);

                if (missionRecipient == MissionRecipient.Both
                        || missionRecipient == MissionRecipient.Artist)
                {
                        Artist artist = await dbContext.Artists.FirstAsync(a => a.UserId == userId);
                        artist.Points += POINTS_PER_QUEST;
                        await dbContext.ArtistPointAwards.AddAsync(new()
                        {
                                PointValue = POINTS_PER_QUEST,
                                ArtistId = artist.Id,
                        });
                }

                if (missionRecipient == MissionRecipient.Both
                        || missionRecipient == MissionRecipient.Reviewer)
                {
                        Reviewer reviewer = await dbContext.Reviewers.FirstAsync(r => r.UserId == userId);
                        reviewer.Points += POINTS_PER_QUEST;
                        await dbContext.ReviewerPointAwards.AddAsync(new()
                        {
                                PointValue = POINTS_PER_QUEST,
                                ReviewerId = reviewer.Id,
                        });
                }

                if (missionRecipient == MissionRecipient.Unknown
                        || (missionRecipient != MissionRecipient.Artist
                                && missionRecipient != MissionRecipient.Reviewer
                                && missionRecipient != MissionRecipient.Both))
                {
                        throw new InvalidEnumArgumentException($"Unknown enum value: {missionRecipient}");
                }

                await dbContext.SaveChangesAsync();
        }
}
