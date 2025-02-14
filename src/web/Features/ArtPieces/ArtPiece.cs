using web.Features.Artists;
using web.Features.Shared.domain;

namespace web.Features.ArtPieces;

public class ArtPiece : AggreggateRoot
{
        public ArtPieceId Id { get; init; } = new ArtPieceId();
        public required string ImagePath { get; init; }
        public required string Description { get; set; }
        public DateTime UploadDate { get; init; } = DateTime.UtcNow;
        public required ArtistId ArtistId { get; init; }
}
