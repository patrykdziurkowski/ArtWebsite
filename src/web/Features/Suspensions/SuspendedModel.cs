namespace web.Features.Suspensions;

public record SuspendedModel
{
        public required string Reason { get; init; }
        public required DateTimeOffset ExpiryDate { get; init; }
}
