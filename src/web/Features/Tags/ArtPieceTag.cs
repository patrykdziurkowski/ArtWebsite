using web.Features.ArtPieces;

namespace web.Features.Tags;

public class ArtPieceTag
{
        public ArtPieceTagId Id { get; init; } = new();
        public required ArtPieceId ArtPieceId { get; init; }
        public required TagId TagId { get; init; }
}
