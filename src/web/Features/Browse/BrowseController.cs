using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Reviewers;

namespace web.Features.Browse;

[Authorize]
public class BrowseController(ReviewerRepository reviewerRepository) : Controller
{
        public async Task<ActionResult> Index()
        {
                Reviewer? currentReviewer = await reviewerRepository.GetByIdAsync(GetUserId())
                        ?? throw new InvalidOperationException("Could not find a reviewer profile for this user.");
                return View(new BrowseModel()
                {
                        ArtPiecesLikedToday = currentReviewer.ActiveLikes.Count(),
                        CurrentReviewerActivePoints = currentReviewer.ActivePoints,
                });
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
