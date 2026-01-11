using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.Reviewers;

namespace web.Features.Missions;

public class MissionManager(
        ApplicationDbContext dbContext,
        IMissionGenerator missionGenerator)
{
        private const int POINTS_PER_QUEST = 25;

        public async Task RecordProgressAsync(MissionType missionType, Guid userId, DateTimeOffset now)
        {
                MissionType[] missions = missionGenerator.GetMissions(userId, now);
                if (!missions.Contains(missionType))
                {
                        return;
                }

                bool missionJustFinished = false;
                int maxMissionProgressCount = missionType.GetMaxProgressCount();
                MissionProgress? missionProgress = await dbContext.MissionProgresses
                        .FirstOrDefaultAsync(mp => mp.UserId == userId);
                if (missionProgress is not null
                        && missionProgress.Date.Date != now.Date)
                {
                        dbContext.MissionProgresses.Remove(missionProgress);
                        missionProgress = null;
                }

                if (missionProgress is null)
                {
                        missionProgress = new()
                        {
                                MissionType = missionType,
                                UserId = userId,
                                Date = now,
                        };
                        await dbContext.AddAsync(missionProgress);
                        missionJustFinished = missionProgress.Count >= maxMissionProgressCount;
                }
                else if (missionProgress.Count < maxMissionProgressCount)
                {
                        missionProgress.Count++;
                        missionJustFinished = missionProgress.Count >= maxMissionProgressCount;
                }

                if (missionJustFinished)
                {
                        await AssignPointsForMissionCompletionAsync(missionType, userId);
                }

                await dbContext.SaveChangesAsync();
        }

        private async Task AssignPointsForMissionCompletionAsync(MissionType missionType, Guid userId)
        {
                MissionRecipient missionRecipient = MissionTypeHelpers.GetRecipient(missionType);

                if (missionRecipient == MissionRecipient.Both
                        || missionRecipient == MissionRecipient.Artist)
                {
                        Artist? artist = await dbContext.Artists.FirstOrDefaultAsync(a => a.UserId == userId);
                        if (artist is not null)
                        {
                                artist.Points += POINTS_PER_QUEST;
                                await dbContext.ArtistPointAwards.AddAsync(new()
                                {
                                        PointValue = POINTS_PER_QUEST,
                                        ArtistId = artist.Id,
                                });
                        }

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
        }
}
