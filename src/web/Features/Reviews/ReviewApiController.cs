using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;

namespace web.Features.Reviews;

[Route("api/review")]
[ApiController]
[Authorize]
public class ReviewApiController : ControllerBase
{
        private const int REVIEWS_TO_LOAD = 10;
        private readonly ReviewArtPieceCommand _reviewArtPieceCommand;
        private readonly ReviewsQuery _reviewsQuery;
        public ReviewApiController(ReviewArtPieceCommand reviewArtPieceCommand,
                ReviewsQuery reviewsQuery)
        {
                _reviewArtPieceCommand = reviewArtPieceCommand;
                _reviewsQuery = reviewsQuery;
        }

        public IActionResult LoadReviews([Range(0, int.MaxValue)] int offset = 0)
        {
                List<Review> reviews = _reviewsQuery.Execute(GetUserId(),
                        REVIEWS_TO_LOAD, offset);
                return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewArtPiece(ReviewArtPieceModel model)
        {
                Review review = await _reviewArtPieceCommand.ExecuteAsync(
                        model.Comment, new ArtPieceId(model.ArtPieceId), GetUserId());
                return Ok(review);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
