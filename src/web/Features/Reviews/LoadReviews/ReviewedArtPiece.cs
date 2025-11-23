namespace web.Features.Reviews.LoadReviews;

public record ReviewerReviewDto
{
        public required DateTimeOffset Date { get; init; }
        public required string ReviewerName { get; init; }
        public required string Comment { get; init; }
        public required int Rating { get; init; }
        public required string ImagePath { get; init; }
}
