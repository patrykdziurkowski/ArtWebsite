namespace web.Features.Missions;

public class TodaysMissionDto
{
        public required string Description { get; init; }
        public required int CurrentProgress { get; init; }
        public required int MaxProgress { get; init; }
}
