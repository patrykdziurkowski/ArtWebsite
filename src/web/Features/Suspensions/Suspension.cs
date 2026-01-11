using web.Features.Shared.domain;

namespace web.Features.Suspensions;

public class Suspension
{
        public SuspensionId Id { get; init; } = new();
        public required Guid IssuingUserId { get; init; }
        public required Guid UserId { get; init; }
        public DateTimeOffset IssuedAt { get; init; } = DateTimeOffset.UtcNow;
        public required string Reason { get; init; }
        public required DateTimeOffset ExpiryDate { get; init; }
}

public class SuspensionId : DomainId;