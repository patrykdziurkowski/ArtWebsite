using web.Features.Shared.domain;

namespace web.Features.Suspensions;

public class Suspension
{
        public SuspensionId Id { get; init; } = new();
        public required Guid IssuingUserId { get; init; }
        public required Guid UserId { get; init; }
        public DateTimeOffset IssuedAt { get; init; } = DateTimeOffset.UtcNow;
        public required TimeSpan Duration { get; init; }
        public required string Reason { get; init; }
        public DateTimeOffset ExpiryDate => IssuedAt.Add(Duration);
}

public class SuspensionId : DomainId;