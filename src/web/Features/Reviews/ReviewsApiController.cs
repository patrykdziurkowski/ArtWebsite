using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Reviews.ReviewArtPiece;

namespace web.Features.Reviews;

[Route("api/review")]
[ApiController]
[Authorize]
public class ReviewsApiController : ControllerBase
{
        private readonly ReviewArtPieceCommand _reviewArtPieceCommand;
        public ReviewsApiController(ReviewArtPieceCommand reviewArtPieceCommand)
        {
                _reviewArtPieceCommand = reviewArtPieceCommand;
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
