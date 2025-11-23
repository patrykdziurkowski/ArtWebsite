using web.Features.Reviewers;

namespace web.Features.PointAwards.Reviewer;

public class ReviewerPointAward
{
        public ReviewerPointAwardId Id { get; init; } = new ReviewerPointAwardId();
        public DateTimeOffset DateAwarded { get; init; } = DateTimeOffset.UtcNow;
        public required ReviewerId ReviewerId { get; init; }
        public required int PointValue { get; init; }
}
