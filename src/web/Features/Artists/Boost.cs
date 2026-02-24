using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Artists;

public class Boost
{
        public BoostId Id { get; init; } = new BoostId();
        public DateTimeOffset Date { get; private init; }
        public DateTimeOffset ExpirationDate { get; private init; }
        public required ArtPieceId? ArtPieceId { get; init; }
        public required ArtistId ArtistId { get; init; }
        public bool IsActive => ExpirationDate >= DateTimeOffset.UtcNow;

        public Boost(DateTimeOffset? date = null)
        {
                date ??= DateTimeOffset.UtcNow;

                Date = date.Value;
                ExpirationDate = date.Value.AddDays(1);
        }

        private Boost()
        {
                // EF
        }
}

public class BoostId : DomainId;
