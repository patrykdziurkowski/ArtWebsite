using web.Features.Shared.domain;

namespace web.Features.Reviewers.UnlikeArtPiece;

public record ArtPieceUnlikedEvent : IDomainEvent
{
        public required Like Like { get; set; }
}
