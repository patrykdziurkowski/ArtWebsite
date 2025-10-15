using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Browse;

[Authorize]
public class BrowseController() : Controller
{
        public ActionResult Index()
        {
                return View();
        }
}
