using FluentResults;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Leaderboard.Reviewer;
using web.Features.Missions;
using web.Features.Reviews;

namespace web.Features.Reviewers.LikeArtPiece;

public class LikeArtPieceCommand(
        ReviewerRepository reviewerRepository,
        ApplicationDbContext dbContext,
        MissionManager missionManager)
{
        public async Task<Result<Like>> ExecuteAsync(
                Guid currentUserId,
                ArtPieceId artPieceId,
                DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                Reviewer? reviewer = await reviewerRepository.GetByIdAsync(currentUserId);
                if (reviewer is null)
                {
                        return Result.Fail("No reviewer profile found for this user id.");
                }

                Review reviewForThisArtPiece = await dbContext.Reviews.FirstOrDefaultAsync(
                        r => r.ArtPieceId == artPieceId && r.ReviewerId == reviewer.Id)
                        ?? throw new InvalidOperationException("Could not like an unreviewed art piece.");;

                if (await dbContext.Likes.AnyAsync(
                        l => l.ArtPieceId == artPieceId && l.ReviewerId == reviewer.Id))
                {
                        throw new InvalidOperationException("Could not like an art piece that was already liked by this reviewer.");
                }

                Result likeResult = reviewer.LikeArtPiece(artPieceId, reviewForThisArtPiece.Id, now);
                if (likeResult.IsFailed)
                {
                        return likeResult;
                }

                dbContext.ReviewerPointAwards.Add(new ReviewerPointAward()
                {
                        ReviewerId = reviewer.Id,
                        PointValue = 15,
                        DateAwarded = reviewer.ActiveLikes.Last().Date,
                });

                ArtPiece likedArtPiece = await dbContext.ArtPieces.FirstAsync(ap => ap.Id == artPieceId);
                likedArtPiece.LikeCount = await dbContext.Likes.CountAsync(l => l.ArtPieceId == artPieceId) + 1;

                await reviewerRepository.SaveAsync(reviewer);

                await missionManager.RecordProgressAsync(MissionType.LikeArt, currentUserId, now.Value);

                return Result.Ok(reviewer.ActiveLikes.Last());
        }
}
