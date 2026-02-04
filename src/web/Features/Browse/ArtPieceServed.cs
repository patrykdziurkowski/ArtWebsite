using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Browse;

public class ArtPieceServed : AggregateRoot
{
        public ArtPieceServedId Id { get; init; } = new();
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public required ArtPieceId ArtPieceId { get; set; }
        public required Guid UserId { get; init; }
        public bool WasSkipped { get; set; } = false;
}

public class ArtPieceServedId : DomainId;