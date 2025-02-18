using FluentResults;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Likes.LikeArtPiece;

public class LikeArtPieceCommand(ApplicationDbContext dbContext,
        LikeLimiterService likeLimiterService)
{
        public async Task<Result<Like>> ExecuteAsync(Guid currentUserId,
                ArtPieceId artPieceId)
        {
                Reviewer reviewer = dbContext.Reviewers
                        .First(r => r.UserId == currentUserId);
                List<Like> mostRecentLikes = dbContext.Likes
                        .OrderByDescending(l => l.Date)
                        .Take(5)
                        .ToList();
                if (likeLimiterService.DailyLikeLimitReached(mostRecentLikes))
                {
                        return Result.Fail("Daily like limit has been reached.");
                }

                Like like = new()
                {
                        ArtPieceId = artPieceId,
                        ReviewerId = reviewer.Id,
                };
                dbContext.Likes.Add(like);
                await dbContext.SaveChangesAsync();
                return Result.Ok(like);
        }
}
