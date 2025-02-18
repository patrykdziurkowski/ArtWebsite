using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Artists;
using web.Features.ArtPieces.Index;
using web.Features.ArtPieces.LoadArtPieces;

namespace web.Features.ArtPieces;

[Route("api/artpiece")]
[ApiController]
[Authorize]
public class ArtPieceApiController(ArtPieceQuery artPieceQuery,
        ArtPiecesQuery artPiecesQuery) : ControllerBase
{
        private const int ART_PIECES_TO_LOAD = 5;

        [HttpGet("/api/artpiece")]
        public IActionResult GetNextArtPiece()
        {
                ArtPiece? artPiece = artPieceQuery.Execute(GetUserId());
                if (artPiece is null)
                {
                        return NoContent();
                }
                return Ok(artPiece);
        }

        [HttpGet("/api/artists/{artistId}/artpieces/")]
        public IActionResult LoadArtPiecesForArtist(Guid artistId,
                [Range(0, int.MaxValue)] int offset = 0)
        {
                List<ArtPiece> artPieces = artPiecesQuery
                        .Execute(new ArtistId { Value = artistId }, ART_PIECES_TO_LOAD,
                                offset);
                return Ok(artPieces);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
