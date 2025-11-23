using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.Reviews.LoadReviews;

public class ReviewerReviewsQuery(ApplicationDbContext dbContext)
{
        public async Task<List<ReviewerReviewDto>> ExecuteAsync(ReviewerId reviewerId,
                int count, int offset = 0)
        {
                return await dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .OrderByDescending(r => r.Date)
                        .Skip(offset)
                        .Take(count)
                        .Join(
                                dbContext.ArtPieces,
                                review => review.ArtPieceId,
                                artPiece => artPiece.Id,
                                (review, artPiece) => new { review, artPiece }
                        )
                        .Join(
                                dbContext.Reviewers,
                                x => x.review.ReviewerId,
                                reviewer => reviewer.Id,
                                (x, reviewer) => new ReviewerReviewDto
                                {
                                        Date = x.review.Date,
                                        Comment = x.review.Comment,
                                        Rating = x.review.Rating.Value,
                                        ImagePath = x.artPiece.ImagePath,
                                        ReviewerName = reviewer.Name
                                }
                        )
                        .ToListAsync();
        }

}
