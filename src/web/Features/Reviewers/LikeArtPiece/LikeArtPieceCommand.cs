using FluentResults;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Reviewers.LikeArtPiece;

public class LikeArtPieceCommand(ApplicationDbContext dbContext)
{
        public async Task<Result<Like>> ExecuteAsync(Guid currentUserId,
                ArtPieceId artPieceId)
        {
                Reviewer reviewer = await dbContext.Reviewers
                        .Where(r => r.UserId == currentUserId)
                        .Include(r => r.ActiveLikes
                                .Where(l => l.ExpirationDate >= DateTimeOffset.UtcNow))
                        .FirstAsync();
                Result likeResult = reviewer.LikeArtPiece(artPieceId);
                if (likeResult.IsFailed)
                {
                        return likeResult;
                }

                await dbContext.SaveChangesAsync();
                return Result.Ok(reviewer.ActiveLikes.Last());
        }
}
