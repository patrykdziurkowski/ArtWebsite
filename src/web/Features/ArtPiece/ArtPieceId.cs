using System;
using web.features.shared.domain;

namespace web.Features.ArtPiece;

public class ArtPieceId : DomainId
{
        public ArtPieceId() : base(Guid.NewGuid())
        {
        }

        public ArtPieceId(Guid guid) : base(guid)
        {
        }
}
