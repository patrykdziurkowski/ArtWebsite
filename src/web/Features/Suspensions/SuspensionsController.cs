using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Suspensions;

[Authorize(Roles = Constants.ADMIN_ROLE)]
public class SuspensionsController : Controller
{
        public ActionResult Index()
        {
                return View();
        }

}
