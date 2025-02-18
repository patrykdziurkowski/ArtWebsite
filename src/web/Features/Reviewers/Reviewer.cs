using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class Reviewer : AggreggateRoot
{
        public ReviewerId Id { get; init; } = new ReviewerId();
        public required string Name { get; init; }
        public DateTimeOffset JoinDate { get; init; } = DateTimeOffset.UtcNow;
        public int ReviewCount { get; init; } = 0;
        public required Guid UserId { get; init; }
}
