using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
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

                await dbContext.AddAsync(review);
                await dbContext.SaveChangesAsync();
                return review;
        }
}
