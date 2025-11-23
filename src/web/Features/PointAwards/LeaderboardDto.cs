namespace web.Features.PointAwards;

public record LeaderboardDto
{
        public required string Name { get; init; }
        public required int PointsInThatTimeSpan { get; init; }
}
