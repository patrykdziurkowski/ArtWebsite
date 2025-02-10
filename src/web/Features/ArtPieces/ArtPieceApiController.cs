using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces.Index;

namespace web.Features.ArtPieces;

[Route("api/artpiece")]
[ApiController]
[Authorize]
public class ArtPieceApiController : ControllerBase
{
        private readonly ArtPieceQuery _artPieceQuery;

        public ArtPieceApiController(ArtPieceQuery artPieceQuery)
        {
                _artPieceQuery = artPieceQuery;
        }

        public IActionResult Index()
        {
                ArtPiece? artPiece = _artPieceQuery.Execute(GetUserId());
                if (artPiece is null)
                {
                        return NoContent();
                }
                return Ok(artPiece);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
