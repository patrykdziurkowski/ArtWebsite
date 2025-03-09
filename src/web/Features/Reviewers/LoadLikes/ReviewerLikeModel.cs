using web.Features.ArtPieces;

namespace web.Features.Reviewers.LoadLikes;

public class ReviewerLikeModel
{
        public required LikeId Id { get; init; }
        public required DateTimeOffset Date { get; init; }
        public required DateTimeOffset ExpirationDate { get; init; }
        public required ArtPieceId ArtPieceId { get; init; }
        public required ReviewerId ReviewerId { get; init; }
        public required string ImagePath { get; init; }
        public bool IsActive => ExpirationDate >= DateTimeOffset.UtcNow;
}
