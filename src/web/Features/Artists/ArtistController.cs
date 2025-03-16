using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists.DeactivateArtist;
using web.Features.Artists.Index;
using web.Features.Artists.SetupArtist;
using web.Features.Shared;

namespace web.Features.Artists;

[Authorize]
public class ArtistController(
        SetupArtistCommand setupArtistCommand,
        UserManager<IdentityUser<Guid>> userManager,
        DeactivateArtistCommand deactivateArtistCommand,
        ArtistRepository artistRepository) : Controller
{
        public async Task<ActionResult> Index()
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(Setup));
                }

                Artist artist = await artistRepository.GetByUserIdAsync(GetUserId())
                        ?? throw new InvalidOperationException("No artist profile found despite user having the artist role.");
                ArtistProfileModel model = new()
                {
                        Id = artist.Id.Value,
                        Name = artist.Name,
                        Summary = artist.Summary,
                        IsOwner = true,
                };
                return View(model);
        }

        [HttpGet("/Artists/{artistId}")]
        public async Task<ActionResult> GetArtist(Guid artistId)
        {
                Artist? artist = await artistRepository.GetByIdAsync(new ArtistId { Value = artistId });
                if (artist is null)
                {
                        return View("Error", new ErrorViewModel(404,
                                "No artist with such id found."));
                }

                ArtistProfileModel model = new()
                {
                        Id = artist.Id.Value,
                        Name = artist.Name,
                        Summary = artist.Summary,
                        IsOwner = artist.UserId == GetUserId(),
                };
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

                Result<Artist> result = await setupArtistCommand.ExecuteAsync(
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


                await deactivateArtistCommand.ExecuteAsync(GetUserId());
                return Redirect("/");
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }

        private async Task<bool> IsArtistAsync()
        {
                IdentityUser<Guid>? user = await userManager.GetUserAsync(User);
                if (user is null)
                {
                        return false;
                }

                return await userManager.IsInRoleAsync(user, "Artist");
        }

}
