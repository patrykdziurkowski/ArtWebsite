using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Missions;
using web.Features.Reviewers.Index;
using web.Features.Shared;

namespace web.Features.Reviewers;

[Authorize]
public class ReviewerController(
        UserReviewerQuery userReviewerQuery,
        ReviewerQuery reviewerQuery,
        MissionManager missionManager) : Controller
{

        public async Task<ActionResult> Index()
        {
                Reviewer? reviewer = await userReviewerQuery.ExecuteAsync(GetUserId())
                        ?? throw new InvalidOperationException("The current user has no reviewer profile.");
                return View(reviewer);
        }

        [HttpGet("/Reviewers/{reviewerName}")]
        public async Task<ActionResult> GetReviewerProfile(string reviewerName)
        {
                Reviewer? reviewer = await reviewerQuery.ExecuteAsync(reviewerName);
                if (reviewer is null)
                {
                        return View("Error", new ErrorViewModel(404,
                                $"No reviewer profile found with the name '{reviewerName}'."));
                }

                await missionManager.RecordProgressAsync(
                        MissionType.VisitReviewersProfiles,
                        GetUserId(),
                        DateTimeOffset.UtcNow);

                return View(reviewer);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
