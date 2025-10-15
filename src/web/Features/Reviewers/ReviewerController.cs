using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Reviewers.Index;
using web.Features.Shared;

namespace web.Features.Reviewers;

[Authorize]
public class ReviewerController(UserReviewerQuery userReviewerQuery) : Controller
{

        public async Task<ActionResult> Index()
        {
                Reviewer? reviewer = await userReviewerQuery.ExecuteAsync(GetUserId());
                if (reviewer is null)
                {
                        return View("Error", new ErrorViewModel(404,
                                "No reviewer profile found for a given user."));
                }
                return View(reviewer);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
