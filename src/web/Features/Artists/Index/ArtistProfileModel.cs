namespace web.Features.Artists.Index;

public class ArtistProfileModel
{
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Summary { get; init; }
        public required bool IsOwner { get; init; }
}
