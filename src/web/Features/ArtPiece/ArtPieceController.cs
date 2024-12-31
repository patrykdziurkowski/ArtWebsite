using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPiece.UploadArtPiece;

namespace web.features.art_piece;

[Authorize]
public class ArtPieceController : Controller
{
        private readonly UploadArtPieceCommand _uploadArtPieceCommand;
        private readonly UserManager<IdentityUser> _userManager;

        public ArtPieceController(
                UploadArtPieceCommand uploadArtPieceCommand,
                UserManager<IdentityUser> userManager)
        {
                _uploadArtPieceCommand = uploadArtPieceCommand;
                _userManager = userManager;
        }

        public ActionResult Index()
        {
                return View();
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
