using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Suspensions;

[Authorize(Roles = "Admin")]
public class SuspensionsController : Controller
{
        public ActionResult Index()
        {
                return Content("This endpoint is authorized for admin only");
        }

}
