using System.ComponentModel;

namespace web.Features.Missions;

public enum MissionType
{
        Unknown,
        UploadArt,
        BoostArt,
        ReviewArt,
        LikeArt,
        VisitArtistsProfiles,
        VisitReviewersProfiles,
}

public enum MissionRecipient
{
        Unknown,
        Artist,
        Reviewer,
        Both
}

public static class Missions
{
        public static MissionRecipient GetRecipient(MissionType missionType) => missionType switch
        {
                MissionType.UploadArt => MissionRecipient.Artist,
                MissionType.BoostArt => MissionRecipient.Artist,
                MissionType.ReviewArt => MissionRecipient.Reviewer,
                MissionType.LikeArt => MissionRecipient.Reviewer,
                MissionType.VisitArtistsProfiles => MissionRecipient.Both,
                MissionType.VisitReviewersProfiles => MissionRecipient.Both,
                MissionType.Unknown => throw new InvalidEnumArgumentException($"Uninitialized (UNKNOWN) enum value for {nameof(MissionType.Unknown)}"),
                _ => throw new InvalidEnumArgumentException($"Could not map the mission type to which entity it is awarded to for value '{missionType}'"),
        };
}
