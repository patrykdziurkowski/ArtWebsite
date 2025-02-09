using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Reviews;

public class Review : AggreggateRoot
{
        public ReviewId Id { get; init; } = new ReviewId();
        public required string Comment { get; set; }
        public DateTime Date { get; init; } = DateTime.UtcNow;
        public required ArtPieceId ArtPieceId { get; init; }
        public required Guid ReviewerId { get; init; }

}
