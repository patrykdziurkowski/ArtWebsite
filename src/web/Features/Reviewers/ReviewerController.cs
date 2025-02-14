using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.Reviewers.Index;

namespace web.Features.Reviewers;

[Authorize]
public class ReviewerController : Controller
{
        private readonly UserReviewerQuery _userReviewerQuery;

        public ReviewerController(UserReviewerQuery userReviewerQuery)
        {
                _userReviewerQuery = userReviewerQuery;
        }

        public ActionResult Index()
        {
                Reviewer reviewer = _userReviewerQuery.Execute(GetUserId());
                return View(reviewer);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
