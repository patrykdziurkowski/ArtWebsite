namespace web.Features.Artists.Index;

public class ArtistProfileModel
{
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Summary { get; init; }
        public required bool IsOwner { get; init; }
        public required bool IsAdmin { get; init; }
        public required string? BoostedArtPiecePath { get; init; }
        public required DateTimeOffset? BoostExpirationDate { get; init; }
}
