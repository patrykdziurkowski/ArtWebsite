using FluentResults;
using web.Features.ArtPieces;

namespace web.Features.Reviewers.UnlikeArtPiece;

public class UnlikeArtPieceCommand(ReviewerRepository reviewerRepository)
{
        public async Task<Result> ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId)
        {
                Reviewer reviewer = await reviewerRepository.GetByIdAsync(currentUserId)
                        ?? throw new ArgumentException($"No reviewer exists for user with id {currentUserId}");

                Result result = reviewer.UnlikeArtPiece(artPieceId);
                if (result.IsFailed)
                {
                        return result;
                }

                await reviewerRepository.SaveAsync(reviewer);
                return Result.Ok();
        }
}
