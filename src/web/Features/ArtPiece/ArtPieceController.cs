using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPiece;
using web.Features.ArtPiece.Index;
using web.Features.ArtPiece.UploadArtPiece;

namespace web.features.art_piece;

[Authorize]
public class ArtPieceController : Controller
{
        private readonly UploadArtPieceCommand _uploadArtPieceCommand;
        private readonly ArtPiecesQuery _artPiecesQuery;
        private readonly UserManager<IdentityUser> _userManager;

        private const int NUMBER_OF_ART_PIECES_TO_LOAD = 5;

        public ArtPieceController(
                UploadArtPieceCommand uploadArtPieceCommand,
                UserManager<IdentityUser> userManager,
                ArtPiecesQuery artPiecesQuery)
        {
                _uploadArtPieceCommand = uploadArtPieceCommand;
                _userManager = userManager;
                _artPiecesQuery = artPiecesQuery;
        }

        public async Task<ActionResult> Index()
        {
                List<ArtPiece> artPieces = await _artPiecesQuery
                        .ExecuteAsync(NUMBER_OF_ART_PIECES_TO_LOAD);
                return View(new ArtPiecesViewModel(artPieces));
        }

        public async Task<ActionResult> Upload()
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(Index));
                }
                return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(UploadArtPieceModel model)
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(Index));
                }
                await _uploadArtPieceCommand.ExecuteAsync(model.Image, model.Description, GetUserId());
                return RedirectToAction(nameof(Index));
        }

        private string GetUserId()
        {
                return User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
        }

        private async Task<bool> IsArtistAsync()
        {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                        return false;
                }

                return await _userManager.IsInRoleAsync(user, "Artist");
        }

}
