using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Home;

namespace web.Features.Suspensions;

[Authorize]
public class SuspensionsController(ApplicationDbContext dbContext) : Controller
{
        [HttpGet("/Suspended")]
        public async Task<IActionResult> Index()
        {
                Guid currentUserId = GetUserId();
                Suspension? activeSuspension =
                        await dbContext.Suspensions.FirstOrDefaultAsync(s =>
                                s.UserId == currentUserId
                                && s.ExpiryDate > DateTimeOffset.UtcNow);
                if (activeSuspension is null)
                {
                        return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
                }

                return View(new SuspendedModel()
                {
                        ExpiryDate = activeSuspension.ExpiryDate,
                        Reason = activeSuspension.Reason,
                });
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
