using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Artists;
using web.Features.ArtPieces.DeleteArtPiece;
using web.Features.ArtPieces.EditArtPiece;
using web.Features.ArtPieces.LoadArtPieces;
using web.Features.Browse;
using web.Features.Browse.ByTag;
using web.Features.Browse.Index;

namespace web.Features.ArtPieces;

[Route("api/artpiece")]
[ApiController]
[Authorize]
public class ArtPieceApiController(
        ArtPieceQuery artPieceQuery,
        ArtPiecesQuery artPiecesQuery,
        ArtPieceByTagQuery artPiecesByTagQuery,
        EditArtPieceCommand editArtPieceCommand,
        DeleteArtPieceCommand deleteArtPieceCommand,
        ArtPieceDetailsQuery artPieceDetailsQuery,
        RegisterArtPieceServedCommand registerArtPieceServedCommand) : ControllerBase
{
        private const int ART_PIECES_TO_LOAD = 5;

        [HttpGet("/api/artpiece")]
        public async Task<IActionResult> GetNextArtPiece([FromQuery] string? tag)
        {
                Guid currentUserId = GetUserId();
                ArtPieceDto? artPiece;
                if (tag is null)
                {
                        artPiece = await artPieceQuery.ExecuteAsync(currentUserId);
                        await registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPiece?.Id);
                }
                else
                {
                        artPiece = await artPiecesByTagQuery.ExecuteAsync(currentUserId, tag);
                }

                if (artPiece is null)
                {
                        return NoContent();
                }

                return Ok(artPiece);
        }

        [HttpGet("/api/artpieces/{artPieceId}")]
        public async Task<IActionResult> GetArtPiece(Guid artPieceId)
        {
                ArtPieceDto artPiece = await artPieceDetailsQuery.ExecuteAsync(new ArtPieceId() { Value = artPieceId });
                return Ok(artPiece);
        }

        [HttpGet("/api/artists/{artistId}/artpieces/")]
        public async Task<IActionResult> LoadArtPiecesForArtist(Guid artistId,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                List<ArtPiece> artPieces = await artPiecesQuery
                        .ExecuteAsync(new ArtistId { Value = artistId }, ART_PIECES_TO_LOAD, offset);
                return Ok(artPieces);
        }

        [HttpPut("/api/artpieces/{artPieceId}")]
        public async Task<IActionResult> EditArtPiece(Guid artPieceId, EditArtPieceModel model)
        {
                await editArtPieceCommand.ExecuteAsync(
                        GetUserId(),
                        new ArtPieceId() { Value = artPieceId },
                        model.Description);
                return Ok();
        }

        [HttpDelete("/api/artpieces/{artPieceId}")]
        public async Task<IActionResult> DeleteArtPiece(Guid artPieceId)
        {
                await deleteArtPieceCommand.ExecuteAsync(GetUserId(), new ArtPieceId() { Value = artPieceId });
                return NoContent();
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
