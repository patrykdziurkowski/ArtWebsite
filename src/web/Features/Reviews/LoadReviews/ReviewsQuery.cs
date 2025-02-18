using web.Data;
using web.Features.Reviewers;

namespace web.Features.Reviews.LoadReviews;

public class ReviewsQuery(ApplicationDbContext dbContext)
{
        public List<ReviewedArtPiece> Execute(ReviewerId reviewerId,
                int count, int offset = 0)
        {
                return dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .OrderByDescending(r => r.Date)
                        .Skip(offset)
                        .Take(count)
                        .Join(
                                dbContext.ArtPieces,
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
