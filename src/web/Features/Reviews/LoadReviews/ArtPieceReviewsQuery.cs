using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Reviews.LoadReviews;

public class ArtPieceReviewsQuery(ApplicationDbContext dbContext)
{
        public async Task<List<ArtPieceReviewDto>> ExecuteAsync(
                ArtPieceId artPieceId, int count, int offset = 0)
        {
                return await dbContext.Reviews
                        .Where(r => r.ArtPieceId == artPieceId)
                        .Join(
                                dbContext.Reviewers,
                                review => review.ReviewerId,
                                reviewer => reviewer.Id,
                                (review, reviewer) => new ArtPieceReviewDto
                                {
                                        ReviewerName = reviewer.Name,
                                        Rating = review.Rating,
                                        Date = review.Date,
                                        Comment = review.Comment,
                                        Points = reviewer.Points,
                                })
                        .OrderByDescending(dto => dto.Points)
                        .Skip(offset)
                        .Take(count)
                        .ToListAsync();
        }

}
