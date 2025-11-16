using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Reviews.LoadReviews;

public class ArtPieceReviewsQuery(ApplicationDbContext dbContext)
{
        public async Task<List<ArtPieceReviewDto>> ExecuteAsync(Guid currentUserId,
                ArtPieceId artPieceId, int count, int offset = 0)
        {
                ReviewerId thisReviewerId = (await dbContext.Reviewers
                        .FirstAsync(r => r.UserId == currentUserId)).Id;
                bool artPieceWasReviewedByCurrentReviewer = await dbContext.Reviews
                        .AnyAsync(r => r.ArtPieceId == artPieceId && r.ReviewerId == thisReviewerId);
                if (!artPieceWasReviewedByCurrentReviewer)
                {
                        throw new InvalidOperationException("Unable to fetch reviews for this art piece. The current user didn't review this art piece yet.");
                }

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
