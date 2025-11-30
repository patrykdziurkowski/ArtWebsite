using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Leaderboard
{
        [Authorize]
        public class LeaderboardController : Controller
        {
                public ActionResult Index()
                {
                        return View();
                }

        }
}
