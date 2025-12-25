namespace web.Features.Reviews.LoadReviews;

public record ArtPieceReviewDto
{
        public required string ReviewerName { get; init; }
        public required DateTimeOffset Date { get; init; }
        public required string Comment { get; init; }
        public required Rating Rating { get; init; }
        public required int Points { get; init; }
        public required bool IsCurrentUser { get; init; }
}
