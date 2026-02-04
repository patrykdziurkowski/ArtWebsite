using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.Browse;

[Authorize]
public class BrowseController(ApplicationDbContext dbContext) : Controller
{
        public async Task<ActionResult> Index()
        {
                int currentReviewerActivePoints = await dbContext.Reviewers
                        .Where(r => r.UserId == GetUserId())
                        .Select(r => r.ActivePoints)
                        .FirstAsync();
                return View(new BrowseModel()
                {
                        CurrentReviewerActivePoints = currentReviewerActivePoints
                });
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
