using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
namespace web.Features.Reviewers.LoadLikes;

public class LikesQuery(ReviewerRepository reviewerRepository,
        ApplicationDbContext dbContext)
{
        public async Task<List<ReviewerLikeModel>> ExecuteAsync(
                ReviewerId reviewerId, int count, int offset = 0)
        {
                List<Like> likes = await reviewerRepository.GetLikesAsync(reviewerId, count, offset);
                Dictionary<ArtPieceId, ArtPiece> artPieces = await dbContext.ArtPieces
                    .Where(a => likes.Select(l => l.ArtPieceId).Contains(a.Id))
                    .ToDictionaryAsync(a => a.Id);

                return [.. likes
                    .Select(like => new ReviewerLikeModel
                    {
                        Id = like.Id,
                        ReviewerId = reviewerId,
                        ReviewId = like.ReviewId,
                        ArtPieceId = like.ArtPieceId,
                        ImagePath = artPieces[like.ArtPieceId].ImagePath,
                        ExpirationDate = like.ExpirationDate,
                        Date = like.Date,
                    })];
        }
}
