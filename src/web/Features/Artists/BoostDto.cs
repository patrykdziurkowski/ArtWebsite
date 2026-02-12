using web.Features.ArtPieces;

namespace web.Features.Artists;

public class BoostDto
{
        public required BoostId Id { get; init; }
        public required DateTimeOffset Date { get; init; }
        public required DateTimeOffset ExpirationDate { get; init; }
        public required ArtPieceId ArtPieceId { get; init; }
        public required string ImagePath { get; init; }
        public required ArtistId ArtistId { get; init; }
        public bool IsActive => ExpirationDate >= DateTimeOffset.UtcNow;
}
