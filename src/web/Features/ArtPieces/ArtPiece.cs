using web.Features.Artists;
using web.Features.Reviews;
using web.Features.Shared.domain;

namespace web.Features.ArtPieces;

public class ArtPiece : AggregateRoot
{
        public ArtPieceId Id { get; init; } = new ArtPieceId();
        public required string ImagePath { get; init; }
        public required string Description { get; set; }
        public int LikeCount { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;
        public Rating AverageRating { get; set; } = Rating.Empty;
        public DateTimeOffset UploadDate { get; init; } = DateTimeOffset.UtcNow;
        public required ArtistId ArtistId { get; init; }
}

public class ArtPieceId : DomainId;
