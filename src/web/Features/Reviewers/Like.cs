using web.Features.ArtPieces;

namespace web.Features.Reviewers;

public class Like
{
        public LikeId Id { get; init; } = new LikeId();
        public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
        public required ArtPieceId ArtPieceId { get; init; }
        public bool IsActive => Date.AddHours(24).CompareTo(DateTimeOffset.UtcNow) > 0; // Last 24 hours
}
