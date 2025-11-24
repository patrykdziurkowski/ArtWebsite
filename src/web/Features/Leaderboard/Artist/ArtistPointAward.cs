using web.Features.Artists;

namespace web.Features.Leaderboard.Artist;

public class ArtistPointAward
{
        public ArtistPointAwardId Id { get; init; } = new ArtistPointAwardId();
        public DateTimeOffset DateAwarded { get; init; } = DateTimeOffset.UtcNow;
        public required ArtistId ArtistId { get; init; }
        public required int PointValue { get; init; }
}
