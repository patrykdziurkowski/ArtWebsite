using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Browse.SkipArtPiece;

namespace web.Features.Browse
{
    [Authorize]
    [ApiController]
    public class BrowseApiController(SkipArtPieceCommand skipArtPieceCommand) : ControllerBase
    {
        [HttpDelete("/api/artpieces/current")]
        public async Task<IActionResult> SkipArtPiece()
        {
            Result result = await skipArtPieceCommand.ExecuteAsync(GetUserId());
            if (result.IsFailed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    errors = result.Errors
                });
            }

            return Ok();
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
    }
}
