using FluentResults;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Reviewers.LikeArtPiece;

public class LikeArtPieceCommand
{
        private readonly ApplicationDbContext _dbContext;

        public LikeArtPieceCommand(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public async Task<Result<Like>> ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId)
        {
                Reviewer reviewer = _dbContext.Reviewers
                        .First(r => r.UserId == currentUserId);
                Result result = reviewer.LikeArtPiece(artPieceId);
                if (result.IsFailed)
                {
                        return result;
                }
                await _dbContext.SaveChangesAsync();
                return Result.Ok(reviewer.Likes.Last());
        }
}
