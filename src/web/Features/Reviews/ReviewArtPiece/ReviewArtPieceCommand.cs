using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.PointAwards.Reviewer;
using web.Features.Reviewers;

namespace web.Features.Reviews.ReviewArtPiece;

public class ReviewArtPieceCommand(ApplicationDbContext dbContext)
{
        private const int POINTS_PER_REVIEW = 10;

        public async Task<Review> ExecuteAsync(string comment,
                int rating, ArtPieceId artPieceId, Guid userId)
        {
                Reviewer reviewer = await dbContext.Reviewers
                        .FirstAsync(r => r.UserId == userId);

                Review review = new()
                {
                        Comment = comment,
                        Rating = new Rating(rating),
                        ArtPieceId = artPieceId,
                        ReviewerId = reviewer.Id,
                };

                reviewer.Points += POINTS_PER_REVIEW;
                await dbContext.PointAwards.AddAsync(new PointAward()
                {
                        ReviewerId = reviewer.Id,
                        PointValue = POINTS_PER_REVIEW,
                });

                ArtPiece artPieceToReview = dbContext.ArtPieces.First(a => a.Id == artPieceId);

                var reviewsQuery = dbContext.Reviews
                        .Where(r => r.ArtPieceId == artPieceId);
                int countOfReviews = (await reviewsQuery.CountAsync()) + 1;
                int sumOfRatingValues = (await reviewsQuery.SumAsync(r => r.Rating)) + review.Rating;

                artPieceToReview.AverageRating = new Rating((double)sumOfRatingValues / sumOfRatingValues);

                await dbContext.AddAsync(review);
                await dbContext.SaveChangesAsync();
                return review;
        }
}
