using FluentResults;
using web.Features.ArtPieces;

namespace web.Features.Reviewers.LikeArtPiece;

public class LikeArtPieceCommand(ReviewerRepository reviewerRepository)
{
        public async Task<Result<Like>> ExecuteAsync(Guid currentUserId,
                ArtPieceId artPieceId)
        {
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
                return Result.Ok(reviewer.ActiveLikes.Last());
        }
}
