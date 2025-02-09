using web.Features.Shared.domain;

namespace web.Features.ArtPieces;

public class ArtPieceId : DomainId
{
        public ArtPieceId() : base(Guid.NewGuid())
        {
        }

        public ArtPieceId(Guid guid) : base(guid)
        {
        }
}
