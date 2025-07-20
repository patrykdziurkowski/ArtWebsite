namespace web.Features.Artists;

public class BoostDto
{
        public BoostId Id { get; init; } = new BoostId();
        public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpirationDate { get; init; } = DateTimeOffset.UtcNow.AddDays(1);
        public required string ImagePath { get; init; }
        public required ArtistId ArtistId { get; init; }
        public bool IsActive => ExpirationDate >= DateTimeOffset.UtcNow;
}
