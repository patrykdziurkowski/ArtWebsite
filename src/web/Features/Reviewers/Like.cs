using web.Features.ArtPieces;

namespace web.Features.Reviewers;

public class Like
{
        public LikeId Id { get; init; } = new LikeId();
        public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpirationDate { get; init; } = DateTimeOffset.UtcNow.AddDays(1);
        public required ArtPieceId ArtPieceId { get; init; }
        public required ReviewerId ReviewerId { get; init; }
        public bool IsActive => ExpirationDate <= DateTimeOffset.UtcNow;

}
