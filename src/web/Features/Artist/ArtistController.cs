using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.data;
using web.features.artist.DeactivateArtist;
using web.features.artist.Index;
using web.features.artist.SetupArtist;
using web.Models;

namespace web.features.artist
{
        [Authorize]
        public class ArtistController : Controller
        {
                private readonly UserManager<IdentityUser> _userManager;
                private readonly ApplicationDbContext _dbContext;
                private readonly SetupArtistCommand _setupArtistCommand;
                private readonly DeactivateArtistCommand _deactivateArtistCommand;

                public ArtistController(
                        SetupArtistCommand setupArtistCommand,
                        UserManager<IdentityUser> userManager,
                        DeactivateArtistCommand deactivateArtistCommand,
                        ApplicationDbContext dbContext)
                {
                        _setupArtistCommand = setupArtistCommand;
                        _userManager = userManager;
                        _deactivateArtistCommand = deactivateArtistCommand;
                        _dbContext = dbContext;
                }

                public async Task<ActionResult> Index()
                {
                        if (await IsArtistAsync() == false)
                        {
                                return RedirectToAction(nameof(Setup));
                        }

                        Artist artist = _dbContext.Artists.First(a => a.OwnerId == GetUserId());
                        ArtistProfileModel model = new(artist.Id.Value,
                                artist.Name, artist.Summary, isOwner: true);
                        return View(model);
                }

                [Route("/Artists/{artistId}")]
                public async Task<ActionResult> Get(Guid artistId)
                {
                        Artist? artist = await _dbContext.Artists
                                .FirstOrDefaultAsync(a => a.Id.Value == artistId);
                        if (artist is null)
                        {
                                return View("Error", new ErrorViewModel(404,
                                        "No artist with such id found."));
                        }

                        ArtistProfileModel model = new(artist.Id.Value,
                                artist.Name, artist.Summary, isOwner: GetUserId() == artist.OwnerId);
                        return View("Index", model);
                }

                public async Task<ActionResult> Setup()
                {
                        if (await IsArtistAsync())
                        {
                                return RedirectToAction(nameof(Index));
                        }

                        return View();
                }

                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<ActionResult> Setup(SetupModel model)
                {
                        if (await IsArtistAsync())
                        {
                                return RedirectToAction(nameof(Index));
                        }

                        Result<Artist> result = await _setupArtistCommand.ExecuteAsync(
                                GetUserId(), model.Name, model.Summary);
                        if (result.IsFailed)
                        {
                                return View(model);
                        }

                        return RedirectToAction(nameof(Index));
                }

                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<ActionResult> Deactivate()
                {
                        if (await IsArtistAsync() == false)
                        {
                                return RedirectToAction(nameof(Setup));
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
