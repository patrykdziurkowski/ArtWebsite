using web.Features.Reviewers;
using web.Features.Shared.domain;

namespace web.Features.Leaderboard.Reviewer;

public class ReviewerPointAward
{
        public ReviewerPointAwardId Id { get; init; } = new ReviewerPointAwardId();
        public DateTimeOffset DateAwarded { get; init; } = DateTimeOffset.UtcNow;
        public required ReviewerId ReviewerId { get; init; }
        public required int PointValue { get; init; }
}

public class ReviewerPointAwardId : DomainId;