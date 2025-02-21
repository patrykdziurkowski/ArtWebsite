using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;

namespace web.Features.Reviews;

[ApiController]
[Authorize]
public class ReviewApiController(ReviewArtPieceCommand reviewArtPieceCommand,
        ReviewsQuery reviewsQuery) : ControllerBase
{
        private const int REVIEWS_TO_LOAD = 10;

        [HttpGet("/api/reviewers/{reviewerId}/reviews")]
        public async Task<IActionResult> LoadReviews(Guid reviewerId,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                List<ReviewedArtPiece> reviews = await reviewsQuery.ExecuteAsync(
                        new ReviewerId { Value = reviewerId }, REVIEWS_TO_LOAD, offset);
                return Ok(reviews);
        }

        [HttpPost("/api/reviews")]
        public async Task<IActionResult> ReviewArtPiece(ReviewArtPieceModel model)
        {
                Review review = await reviewArtPieceCommand.ExecuteAsync(
                        model.Comment, new ArtPieceId { Value = model.ArtPieceId }, GetUserId());
                return Ok(review);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
