using FluentResults;
using Microsoft.AspNetCore.Mvc;
using web.features.artist.SetupArtist;

namespace web.features.artist
{
        public class ArtistController : Controller
        {
                private readonly SetupArtistCommand _setupArtistCommand;

                public ArtistController(SetupArtistCommand setupArtistCommand)
                {
                        _setupArtistCommand = setupArtistCommand;
                }

                public ActionResult Index()
                {
                        return Content("Index page");
                }

                public ActionResult Setup()
                {
                        return View();
                }

                [HttpPost]
                public async Task<ActionResult> Setup(SetupModel model)
                {
                        if (!ModelState.IsValid)
                        {
                                return View(model);
                        }

                        Result<Artist> result = await _setupArtistCommand.ExecuteAsync(model.Name, model.Summary);
                        if (result.IsFailed)
                        {
                                return View(model);
                        }

                        return RedirectToAction("Index");
                }

        }
}
