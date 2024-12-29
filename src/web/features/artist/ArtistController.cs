using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.features.artist.SetupArtist;

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

                public async Task<ActionResult> Index()
                {
                        if (await IsArtist() == false)
                        {
                                return Redirect("/Artist/Setup");
                        }

                        return Content("Index page");
                }

                public async Task<ActionResult> Setup()
                {
                        if (await IsArtist())
                        {
                                return Redirect("/Artist/Index");
                        }

                        return View();
                }

                [HttpPost]
                public async Task<ActionResult> Setup(SetupModel model)
                {
                        if (await IsArtist())
                        {
                                return Redirect("/Artist/Index");
                        }

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
                        return Redirect("/Artist/Index");
                }

                private async Task<bool> IsArtist()
                {
                        IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
                        if (user is null)
                        {
                                return false;
                        }

                        return await _userManager.IsInRoleAsync(user, "Artist");
                }

        }
}
