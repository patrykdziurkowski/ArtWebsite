using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Artists.BoostArtPiece;
using web.Features.Artists.UpdateArtistProfile;
using web.Features.ArtPieces;

namespace web.Features.Artists;

[Authorize]
[ApiController]
public class ArtistApiController(ArtistRepository artistRepository,
        BoostArtPieceCommand boostArtPieceCommand) : ControllerBase
{
        [HttpPut("/api/artist")]
        public async Task<IActionResult> UpdateArtistProfile(UpdateArtistProfileModel model)
        {
                Artist? artist = await artistRepository.GetByUserIdAsync(GetUserId());
                if (artist is null)
                {
                        return Forbid();
                }

                artist.Name = model.Name;
                artist.Summary = model.Summary;
                await artistRepository.SaveChangesAsync(artist);
                return Ok();
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
