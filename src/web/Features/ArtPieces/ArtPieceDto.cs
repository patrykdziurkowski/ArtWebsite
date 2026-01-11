using web.Features.Artists;

namespace web.Features.ArtPieces;

public record ArtPieceDto
{
        public required ArtPieceId Id { get; init; }
        public required string ImagePath { get; init; }
        public required string Description { get; set; }
        public required int AverageRating { get; set; }
        public required DateTimeOffset UploadDate { get; init; }
        public required ArtistId ArtistId { get; init; }
        public required Guid ArtistUserId { get; init; }
        public required string ArtistName { get; init; }
}
