using web.Features.Shared.domain;

namespace web.Features.Artists;

public class Artist : AggreggateRoot
{
        public ArtistId Id { get; init; } = new ArtistId();
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public required Guid UserId { get; init; }
}
