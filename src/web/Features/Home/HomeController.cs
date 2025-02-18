using Microsoft.AspNetCore.Mvc;

namespace web.Features.Home;

public class HomeController : Controller
{
        public IActionResult Index()
        {
                return View();
        }

        public IActionResult Privacy()
        {
                return View();
        }

}
