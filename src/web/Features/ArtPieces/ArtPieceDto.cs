using web.Features.Artists;

namespace web.Features.ArtPieces;

public record ArtPieceDto
{
        public required ArtPieceId Id { get; init; }
        public required string ImagePath { get; init; }
        public required string Description { get; init; }
        public required int AverageRating { get; init; }
        public required DateTimeOffset UploadDate { get; init; }
        public required int LikeCount { get; init; }
        public required ArtistId ArtistId { get; init; }
        public required Guid ArtistUserId { get; init; }
        public required string ArtistName { get; init; }
}
