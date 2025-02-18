using FluentResults;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Reviewers.LikeArtPiece;

public class LikeArtPieceCommand(ApplicationDbContext dbContext)
{
        public async Task<Result<Like>> ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId)
        {
                Reviewer reviewer = dbContext.Reviewers
                        .First(r => r.UserId == currentUserId);
                Result result = reviewer.LikeArtPiece(artPieceId);
                if (result.IsFailed)
                {
                        return result;
                }
                await dbContext.SaveChangesAsync();
                return Result.Ok(reviewer.Likes.Last());
        }
}
