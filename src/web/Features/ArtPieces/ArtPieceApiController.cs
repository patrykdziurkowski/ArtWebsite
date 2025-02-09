using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces.Index;

namespace web.Features.ArtPieces
{
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

                public ArtPiece? Index()
                {
                        ArtPiece? artPiece = _artPieceQuery.Execute();
                        return artPiece;
                }
        }
}
