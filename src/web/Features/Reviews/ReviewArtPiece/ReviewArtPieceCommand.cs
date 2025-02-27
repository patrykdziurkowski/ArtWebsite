using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Reviews.ReviewArtPiece;

public class ReviewArtPieceCommand(ApplicationDbContext dbContext)
{
        public async Task<Review> ExecuteAsync(string comment,
                int rating, ArtPieceId artPieceId, Guid userId)
        {
                ReviewerId reviewerId = (await dbContext.Reviewers
                        .FirstAsync(r => r.UserId == userId)).Id;

                Review review = new()
                {
                        Comment = comment,
                        Rating = new Rating(rating),
                        ArtPieceId = artPieceId,
                        ReviewerId = reviewerId,
                };
                await dbContext.AddAsync(review);
                await dbContext.SaveChangesAsync();
                return review;
        }
}
