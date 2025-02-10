using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Reviews.ReviewArtPiece;

public class ReviewArtPieceCommand
{
        private readonly ApplicationDbContext _dbContext;
        public ReviewArtPieceCommand(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public async Task<Review> ExecuteAsync(string comment,
                ArtPieceId artPieceId, Guid reviewerId)
        {
                Review review = new()
                {
                        Comment = comment,
                        ArtPieceId = artPieceId,
                        ReviewerId = reviewerId,
                };
                await _dbContext.AddAsync(review);
                await _dbContext.SaveChangesAsync();
                return review;
        }
}
