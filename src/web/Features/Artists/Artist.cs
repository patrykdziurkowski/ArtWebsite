using FluentResults;
using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Artists;

public class Artist : AggreggateRoot
{
        public ArtistId Id { get; init; } = new ArtistId();
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public int Points { get; set; } = 0;
        private Boost? _activeBoost;
        public Boost? ActiveBoost
        {
                get => _activeBoost;
                init
                {
                        _activeBoost = value;
                }
        }
        public required Guid UserId { get; init; }

        public Result BoostArtPiece(ArtPieceId artPieceId, ArtistId artPieceArtistId)
        {
                if (artPieceArtistId != Id)
                {
                        return Result.Fail("You may not boost another artist's art piece.");
                }

                if (ActiveBoost is not null && ActiveBoost.IsActive)
                {
                        return Result.Fail("An art piece is already boosted.");
                }

                _activeBoost = new()
                {
                        ArtistId = Id,
                        ArtPieceId = artPieceId,
                };
                return Result.Ok();
        }

        public void Deactivate()
        {
                RaiseDomainEvent(new ArtistDeactivatedEvent());
        }
}
