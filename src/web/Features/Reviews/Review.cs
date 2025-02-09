using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Reviews;

public class Review : AggreggateRoot
{
        public required ReviewId Id { get; init; } = new ReviewId();
        public required string Comment { get; set; }
        public required DateTime Date { get; init; } = DateTime.UtcNow;
        public required ArtPieceId ArtPieceId { get; init; }
        public required Guid ReviewerId { get; init; }

}
