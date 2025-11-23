using web.Features.Reviewers;

namespace web.Features.PointAwards.Reviewer;

public class PointAward
{
        public PointAwardId Id { get; init; } = new PointAwardId();
        public DateTimeOffset DateAwarded { get; init; } = DateTimeOffset.UtcNow;
        public required ReviewerId ReviewerId { get; init; }
        public required int PointValue { get; init; }
}
