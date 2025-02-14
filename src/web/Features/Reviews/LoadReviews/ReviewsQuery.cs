using web.Data;

namespace web.Features.Reviews.LoadReviews;

public class ReviewsQuery
{
        private readonly ApplicationDbContext _dbContext;
        public ReviewsQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public List<ReviewedArtPiece> Execute(Guid reviewerId, int count, int offset = 0)
        {
                return _dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .OrderByDescending(r => r.Date)
                        .Skip(offset)
                        .Take(count)
                        .Join(
                                _dbContext.ArtPieces,
                                review => review.ArtPieceId,
                                artPiece => artPiece.Id,
                                (review, artPiece) => new ReviewedArtPiece
                                {
                                        Date = review.Date,
                                        Comment = review.Comment,
                                        ImagePath = artPiece.ImagePath
                                })
                        .ToList();
        }

}
