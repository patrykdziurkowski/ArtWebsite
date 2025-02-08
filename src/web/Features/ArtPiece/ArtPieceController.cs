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
        private readonly ArtPieceQuery _artPieceQuery;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public ArtPieceController(
                UploadArtPieceCommand uploadArtPieceCommand,
                UserManager<IdentityUser<Guid>> userManager,
                ArtPieceQuery artPieceQuery)
        {
                _uploadArtPieceCommand = uploadArtPieceCommand;
                _userManager = userManager;
                _artPieceQuery = artPieceQuery;
        }

        public ActionResult Index()
        {
                ArtPiece? artPiece = _artPieceQuery.Execute();
                return View(artPiece);
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

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }

        private async Task<bool> IsArtistAsync()
        {
                IdentityUser<Guid>? user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                        return false;
                }

                return await _userManager.IsInRoleAsync(user, "Artist");
        }

}
