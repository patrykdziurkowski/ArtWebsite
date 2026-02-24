using web.Features.ArtPieces;
using web.Features.Reviews;
using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class Like
{
        public LikeId Id { get; init; } = new LikeId();
        public DateTimeOffset Date { get; private init; }
        public DateTimeOffset ExpirationDate { get; private init; }
        public required ArtPieceId ArtPieceId { get; init; }
        public required ReviewId ReviewId { get; init; }
        public required ReviewerId ReviewerId { get; init; }
        public bool IsActive => ExpirationDate >= DateTimeOffset.UtcNow;

        public Like(DateTimeOffset? date = null)
        {
                Date = date ?? DateTimeOffset.UtcNow;
                ExpirationDate = Date.AddDays(1);
        }

        private Like()
        {
                // EF
        }

}

public class LikeId : DomainId;
