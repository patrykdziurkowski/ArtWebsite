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

public static class MissionTypeExtensions
{
        public static MissionRecipient GetRecipient(this MissionType missionType) => missionType switch
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

        public static int GetMaxProgressCount(this MissionType missionType) => missionType switch
        {
                MissionType.UploadArt => 1,
                MissionType.BoostArt => 1,
                MissionType.ReviewArt => 4,
                MissionType.LikeArt => 2,
                MissionType.VisitArtistsProfiles => 3,
                MissionType.VisitReviewersProfiles => 3,
                MissionType.Unknown => throw new InvalidEnumArgumentException($"Uninitialized (UNKNOWN) enum value for {nameof(MissionType.Unknown)}"),
                _ => throw new InvalidEnumArgumentException($"Could not map the mission type '{missionType}' to its progress count."),
        };

        public static string GetDescription(this MissionType missionType) => missionType switch
        {
                MissionType.UploadArt => "Upload art pieces",
                MissionType.BoostArt => "Boost your art pieces",
                MissionType.ReviewArt => "Review people's art pieces",
                MissionType.LikeArt => "Like art pieces",
                MissionType.VisitArtistsProfiles => "Visit unique artist's profiles",
                MissionType.VisitReviewersProfiles => "Visit unique reviewer's profiles",
                MissionType.Unknown => throw new InvalidEnumArgumentException($"Uninitialized (UNKNOWN) enum value for {nameof(MissionType.Unknown)}"),
                _ => throw new InvalidEnumArgumentException($"Could not map the mission type '{missionType}' to its description."),
        };
}
