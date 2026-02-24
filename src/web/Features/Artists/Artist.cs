using FluentResults;
using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Artists;

public class Artist : AggregateRoot
{
        public ArtistId Id { get; init; } = new ArtistId();
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public string ProfilePicturePath { get; set; } = Constants.DEFAULT_PROFILE_PICTURE_PATH;
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

        public Result BoostArtPiece(ArtPieceId artPieceId, ArtistId artPieceArtistId, DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                if (artPieceArtistId != Id)
                {
                        return Result.Fail("You may not boost another artist's art piece.");
                }

                if (ActiveBoost is not null && ActiveBoost.IsActive)
                {
                        return Result.Fail("An art piece is already boosted.");
                }

                _activeBoost = new(now.Value)
                {
                        ArtistId = Id,
                        ArtPieceId = artPieceId,
                };

                Points += 20;

                return Result.Ok();
        }

        public void Deactivate()
        {
                RaiseDomainEvent(new ArtistDeactivatedEvent());
        }
}

public class ArtistId : DomainId;
