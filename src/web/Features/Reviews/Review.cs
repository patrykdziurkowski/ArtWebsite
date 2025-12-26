using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Shared.domain;

namespace web.Features.Reviews;

public class Review : AggregateRoot
{
        public ReviewId Id { get; init; } = new ReviewId();
        public required string Comment { get; set; }
        public required Rating Rating { get; set; }
        public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
        public required ArtPieceId ArtPieceId { get; init; }
        public required ReviewerId ReviewerId { get; init; }
}
