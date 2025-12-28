using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Tags;

public class ArtPieceTag
{
        public ArtPieceTagId Id { get; init; } = new();
        public required ArtPieceId ArtPieceId { get; init; }
        public required TagId TagId { get; init; }
}

public class ArtPieceTagId : DomainId;
