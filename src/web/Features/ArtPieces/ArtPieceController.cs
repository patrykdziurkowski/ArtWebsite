using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.Features.Artists;
namespace web.Features.ArtPieces;

[Authorize]
public class ArtPieceController(UserManager<IdentityUser<Guid>> userManager) : Controller
{
        public async Task<ActionResult> Upload()
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(ArtistController.Index), "Artist");
                }

                return View();
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
