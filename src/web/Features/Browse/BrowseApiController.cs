using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Browse.ByTag;
using web.Features.Browse.Index;
using web.Features.Browse.SkipArtPiece;

namespace web.Features.Browse
{
    [Authorize]
    [ApiController]
    public class BrowseApiController(
        SkipArtPieceCommand skipArtPieceCommand,
        ArtPieceQuery artPieceQuery,
        ArtPieceByTagQuery artPieceByTagQuery) : ControllerBase
    {
        [HttpDelete("/api/artpieces/current")]
        public async Task<IActionResult> SkipArtPiece(Guid currentArtPieceId, string? tag = null)
        {
            Guid currentUserId = GetUserId();
            Result result = await skipArtPieceCommand.ExecuteAsync(currentUserId);
            if (result.IsFailed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    errors = result.Errors
                });
            }

            if (tag is null)
            {
                ArtPieceDto? artPiece = await artPieceQuery.ExecuteAsync(
                    currentUserId,
                    exceptArtPieceId: new ArtPieceId() { Value = currentArtPieceId });
                return Ok(artPiece);
            }
            else
            {
                ArtPieceDto? artPiece = await artPieceByTagQuery.ExecuteAsync(
                    currentUserId,
                    tag,
                    exceptArtPieceId: new ArtPieceId() { Value = currentArtPieceId });
                return Ok(artPiece);
            }
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
    }
}
