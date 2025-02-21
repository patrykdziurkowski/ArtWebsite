using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.Reviews.LoadReviews;

public class ReviewsQuery(ApplicationDbContext dbContext)
{
        public async Task<List<ReviewedArtPiece>> ExecuteAsync(ReviewerId reviewerId,
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
                                (review, artPiece) => new ReviewedArtPiece
                                {
                                        Date = review.Date,
                                        Comment = review.Comment,
                                        ImagePath = artPiece.ImagePath
                                })
                        .ToListAsync();
        }

}
