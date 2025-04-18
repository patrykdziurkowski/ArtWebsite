using web.Features.Artists;
using web.Features.Shared.domain;

namespace web.Features.ArtPieces;

public class ArtPiece : AggreggateRoot
{
        public ArtPieceId Id { get; init; } = new ArtPieceId();
        public required string ImagePath { get; init; }
        public required string Description { get; set; }
        public int AverageRating { get; set; } = 0;
        public DateTimeOffset UploadDate { get; init; } = DateTimeOffset.UtcNow;
        public required ArtistId ArtistId { get; init; }
}
