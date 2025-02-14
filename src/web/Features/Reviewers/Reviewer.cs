using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class Reviewer : AggreggateRoot
{
        public ReviewerId Id { get; init; } = new ReviewerId();
        public required string Name { get; init; }
        public DateTime JoinDate { get; init; } = DateTime.UtcNow;
        public required Guid OwnerId { get; init; }
}
