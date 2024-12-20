using System;
using web.features.shared.domain;

namespace web.features.artist;

public class ArtistId : DomainId
{
        public ArtistId() : base(Guid.NewGuid())
        {
        }

        public ArtistId(Guid guid) : base(guid)
        {
        }
}
