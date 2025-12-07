using web.Features.Shared.domain;

namespace web.Features.Missions;

public class MissionProgressId : DomainId;

public class MissionProgress
{
        public MissionProgressId Id { get; init; } = new();
        public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
        public required MissionType MissionType { get; init; }
        public int Count { get; set; } = 1;
        public required Guid UserId { get; init; }
}
