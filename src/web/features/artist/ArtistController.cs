using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.features.artist.DeactivateArtist;
using web.features.artist.SetupArtist;

namespace web.features.artist
{
        [Authorize]
        public class ArtistController : Controller
        {
                private readonly UserManager<IdentityUser> _userManager;
                private readonly SetupArtistCommand _setupArtistCommand;
                private readonly DeactivateArtistCommand _deactivateArtistCommand;

                public ArtistController(
                        SetupArtistCommand setupArtistCommand,
                        UserManager<IdentityUser> userManager,
                        DeactivateArtistCommand deactivateArtistCommand)
                {
                        _setupArtistCommand = setupArtistCommand;
                        _userManager = userManager;
                        _deactivateArtistCommand = deactivateArtistCommand;
                }

                public async Task<ActionResult> Index()
                {
                        if (await IsArtistAsync() == false)
                        {
                                return Redirect("/Artist/Setup");
                        }

                        return Content("Index page");
                }

                public async Task<ActionResult> Setup()
                {
                        if (await IsArtistAsync())
                        {
                                return Redirect("/Artist/Index");
                        }

                        return View();
                }

                [HttpPost]
                public async Task<ActionResult> Setup(SetupModel model)
                {
                        if (await IsArtistAsync())
                        {
                                return Redirect("/Artist/Index");
                        }

                        if (!ModelState.IsValid)
                        {
                                return View(model);
                        }

                        Result<Artist> result = await _setupArtistCommand.ExecuteAsync(
                                GetUserId(), model.Name, model.Summary);
                        if (result.IsFailed)
                        {
                                return View(model);
                        }

                        return Redirect("/Artist/Index");
                }

                [HttpDelete]
                public async Task<ActionResult> Deactivate()
                {
                        if (await IsArtistAsync() == false)
                        {
                                return Redirect("/Artist/Setup");
                        }


                        await _deactivateArtistCommand.ExecuteAsync(GetUserId());
                        return Redirect("/");
                }

                private string GetUserId()
                {
                        return User.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                }

                private async Task<bool> IsArtistAsync()
                {
                        IdentityUser? user = await _userManager.GetUserAsync(User);
                        if (user is null)
                        {
                                return false;
                        }

                        return await _userManager.IsInRoleAsync(user, "Artist");
                }

        }
}
