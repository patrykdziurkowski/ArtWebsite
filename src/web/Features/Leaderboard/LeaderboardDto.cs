namespace web.Features.Leaderboard;

public record LeaderboardDto
{
        public required string Name { get; init; }
        public required int PointsInThatTimeSpan { get; init; }
}
