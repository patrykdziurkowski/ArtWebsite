using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.Data;
using web.Features.Artists.BoostArtPiece;
using web.Features.Artists.UpdateArtistProfile;
using web.Features.ArtPieces;

namespace web.Features.Artists;

[Authorize]
[ApiController]
public class ArtistApiController(
        BoostArtPieceCommand boostArtPieceCommand,
        UpdateArtistCommand updateArtistCommand
        ) : ControllerBase
{
        [HttpPut("/api/artist")]
        public async Task<IActionResult> UpdateArtistProfile(UpdateArtistProfileModel model)
        {
                Result result = await updateArtistCommand.ExecuteAsync(
                        GetUserId(),
                        new ArtistId() { Value = model.ArtistId },
                        model.Name,
                        model.Summary,
                        model.Image);

                if (result.IsFailed)
                {
                        return Forbid();
                }
                else
                {
                        return Ok();
                }
        }

        [HttpPost("/api/artist/boost")]
        public async Task<IActionResult> BoostArtPiece([FromBody] BoostArtPieceRequest input)
        {
                Result<BoostDto> result = await boostArtPieceCommand
                        .ExecuteAsync(GetUserId(), new ArtPieceId { Value = input.ArtPieceId });
                if (result.IsFailed)
                {
                        return Forbid();
                }

                return CreatedAtAction(nameof(ArtistController.Index), result.Value);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
