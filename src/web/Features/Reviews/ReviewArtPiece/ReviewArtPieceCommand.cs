using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Browse;
using web.Features.Leaderboard.Reviewer;
using web.Features.Missions;
using web.Features.Reviewers;

namespace web.Features.Reviews.ReviewArtPiece;

public class ReviewArtPieceCommand(
        ApplicationDbContext dbContext,
        MissionManager missionManager)
{
        private const int POINTS_PER_REVIEW = 10;

        public async Task<Review> ExecuteAsync(string comment,
                int rating, ArtPieceId artPieceId, Guid currentUserId,
                TimeSpan reviewCooldown, DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                Reviewer reviewer = await dbContext.Reviewers
                        .FirstAsync(r => r.UserId == currentUserId);

                if (await dbContext.Reviews.AnyAsync(
                        r => r.ArtPieceId == artPieceId && r.ReviewerId == reviewer.Id))
                {
                        throw new InvalidOperationException("This reviewer has already reviewed this art piece.");
                }

                ArtPieceServed artPieceServed = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId)
                                ?? throw new InvalidOperationException("Attempted to review an art piece that wasn't served.");

                if (now.Value.Subtract(artPieceServed.Date) < reviewCooldown)
                {
                        throw new InvalidOperationException("Attempted to review an art piece too early!");
                }

                Review review = new()
                {
                        Comment = comment,
                        Rating = new Rating(rating),
                        ArtPieceId = artPieceId,
                        ReviewerId = reviewer.Id,
                };

                reviewer.Points += POINTS_PER_REVIEW;
                reviewer.ActivePoints += POINTS_PER_REVIEW;
                await dbContext.ReviewerPointAwards.AddAsync(new ReviewerPointAward()
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
                artPieceToReview.ReviewCount = await dbContext.Reviews.CountAsync(r => r.ArtPieceId == artPieceToReview.Id) + 1;

                await dbContext.AddAsync(review);
                await dbContext.SaveChangesAsync();

                await missionManager.RecordProgressAsync(MissionType.ReviewArt, currentUserId, now.Value);

                return review;
        }
}
