using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Browse;
using web.Features.Reviewers;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;

namespace web.Features.Reviews;

[ApiController]
[Authorize]
public class ReviewApiController(
        ReviewArtPieceCommand reviewArtPieceCommand,
        ReviewerReviewsQuery reviewsForReviewerQuery,
        ArtPieceReviewsQuery reviewsForArtPieceQuery,
        RegisterArtPieceServedCommand registerArtPieceServedCommand,
        ApplicationDbContext dbContext) : ControllerBase
{
        private const int REVIEWS_TO_LOAD_FOR_REVIEWER = 10;
        private const int REVIEWS_TO_LOAD_FOR_ART_PIECE = 10;

        [HttpGet("/api/reviewers/{reviewerId}/reviews")]
        public async Task<IActionResult> LoadReviewsForReviewer(Guid reviewerId,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                List<ReviewerReviewDto> reviews = await reviewsForReviewerQuery.ExecuteAsync(
                        new ReviewerId { Value = reviewerId }, REVIEWS_TO_LOAD_FOR_REVIEWER, offset);
                return Ok(reviews);
        }

        [HttpGet("/api/artPieces/{artPieceId}/reviews")]
        public async Task<IActionResult> LoadReviewsForArtPiece(Guid artPieceId,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                List<ArtPieceReviewDto> reviews = await reviewsForArtPieceQuery.ExecuteAsync(
                        GetUserId(), new ArtPieceId { Value = artPieceId }, REVIEWS_TO_LOAD_FOR_ART_PIECE, offset);
                return Ok(reviews);
        }

        [HttpPost("/api/reviews")]
        public async Task<IActionResult> ReviewArtPiece(ReviewArtPieceModel model)
        {
                Guid currentUserId = GetUserId();
                Review review = await reviewArtPieceCommand.ExecuteAsync(
                        model.Comment, model.Rating,
                        new ArtPieceId { Value = model.ArtPieceId }, currentUserId);

                ArtPieceServed? aps = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);
                if (aps is not null)
                {
                        await registerArtPieceServedCommand.ExecuteAsync(currentUserId, null);
                }

                return Ok(review);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
