using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.Features.Artists;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Browse;

namespace web.Features.ArtPieces;

[Authorize]
public class ArtPieceController(UploadArtPieceCommand uploadArtPieceCommand,
        UserManager<IdentityUser<Guid>> userManager) : Controller
{
        public async Task<ActionResult> Upload()
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(ArtistController.Index), "Artist");
                }

                return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(UploadArtPieceModel model)
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(ArtistController.Index), "Artist");
                }

                await uploadArtPieceCommand.ExecuteAsync(model.Image, model.Description, GetUserId());
                return RedirectToAction(nameof(BrowseController.Index), "Browse");
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }

        private async Task<bool> IsArtistAsync()
        {
                IdentityUser<Guid>? user = await userManager.GetUserAsync(User);
                if (user is null)
                {
                        return false;
                }

                return await userManager.IsInRoleAsync(user, Constants.ARTIST_ROLE);
        }

}
