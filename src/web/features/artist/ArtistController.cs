using Microsoft.AspNetCore.Mvc;
using web.features.artist.SetupArtist;

namespace web.features.artist
{
        public class ArtistController : Controller
        {
                public ActionResult Setup()
                {
                        return View();
                }

                // [HttpPost]
                // public ActionResult Setup(SetupModel model)
                // {

                // }

        }
}
