using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Reviewers;

[Authorize]
public class ReviewerController : Controller
{
        public ActionResult Index()
        {
                return View();
        }
}
