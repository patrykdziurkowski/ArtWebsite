using web.Features.Shared.domain;

namespace web.Features.Artists;

public class ArtistId : DomainId
{
        public ArtistId() : base(Guid.NewGuid())
        {
        }

        public ArtistId(Guid guid) : base(guid)
        {
        }
}
