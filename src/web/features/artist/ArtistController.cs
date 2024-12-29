using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using web.features.artist.SetupArtist;
using web.features.shared;

namespace web.features.artist
{
        [Authorize]
        public class ArtistController : Controller
        {
                private readonly UserManager<IdentityUser> _userManager;
                private readonly SetupArtistCommand _setupArtistCommand;

                public ArtistController(SetupArtistCommand setupArtistCommand, UserManager<IdentityUser> userManager)
                {
                        _setupArtistCommand = setupArtistCommand;
                        _userManager = userManager;
                }

                [AuthorizeOrRedirect("Artist", "/Artist/Setup")]
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

                        IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
                        if (user is null)
                        {
                                return Unauthorized();
                        }

                        await _userManager.AddToRoleAsync(user, "Artist");
                        return RedirectToAction("Index");
                }

        }
}
