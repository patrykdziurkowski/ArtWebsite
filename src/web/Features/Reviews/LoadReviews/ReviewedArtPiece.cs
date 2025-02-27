namespace web.Features.Reviews.LoadReviews;

public class ReviewedArtPiece
{
        public required DateTimeOffset Date { get; init; }
        public required string Comment { get; init; }
        public required int Rating { get; init; }
        public required string ImagePath { get; init; }
}
