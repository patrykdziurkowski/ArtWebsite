using FluentResults;
using web.Features.ArtPieces;
using web.Features.Missions;

namespace web.Features.Reviewers.LikeArtPiece;

public class LikeArtPieceCommand(
        ReviewerRepository reviewerRepository,
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

                Result likeResult = reviewer.LikeArtPiece(artPieceId);
                if (likeResult.IsFailed)
                {
                        return likeResult;
                }

                await reviewerRepository.SaveAsync(reviewer);

                await missionManager.RecordProgressAsync(MissionType.LikeArt, currentUserId, now.Value);

                return Result.Ok(reviewer.ActiveLikes.Last());
        }
}
