using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.Data;
using web.Features.Artists.DeactivateArtist;
using web.Features.Artists.Index;
using web.Features.Artists.SetupArtist;
using web.Features.ArtPieces;
using web.Features.Missions;
using web.Features.Shared;

namespace web.Features.Artists;

[Authorize]
public class ArtistController(
        SetupArtistCommand setupArtistCommand,
        UserManager<IdentityUser<Guid>> userManager,
        DeactivateArtistCommand deactivateArtistCommand,
        ArtistRepository artistRepository,
        MissionManager missionManager,
        ApplicationDbContext dbContext) : Controller
{
        public async Task<ActionResult> Index()
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(Setup));
                }

                Artist artist = await artistRepository.GetByUserIdAsync(GetUserId())
                        ?? throw new InvalidOperationException("No artist profile found despite user having the artist role.");
                ArtPiece? boostedArtPiece = artist.ActiveBoost?.ArtPieceId is ArtPieceId boostedArtPieceId
                        ? dbContext.ArtPieces.FirstOrDefault(a => a.Id == boostedArtPieceId)
                        : null;
                ArtistProfileModel model = new()
                {
                        Id = artist.Id.Value,
                        Name = artist.Name,
                        Summary = artist.Summary,
                        IsOwner = true,
                        IsAdmin = await userManager.IsInRoleAsync(
                                (await userManager.FindByIdAsync(GetUserId().ToString()))!,
                                Constants.ADMIN_ROLE),
                        BoostedArtPiecePath = boostedArtPiece?.ImagePath,
                        BoostExpirationDate = artist.ActiveBoost?.ExpirationDate,
                };
                return View(model);
        }

        [HttpGet("/Artists/{artistName}")]
        public async Task<ActionResult> GetArtist(string artistName)
        {
                Artist? artist = await artistRepository.GetByNameAsync(artistName);
                if (artist is null)
                {
                        return View("Error", new ErrorViewModel(404,
                                "No artist with such id found."));
                }

                ArtPiece? boostedArtPiece = artist.ActiveBoost?.ArtPieceId is ArtPieceId boostedArtPieceId
                        ? dbContext.ArtPieces.FirstOrDefault(a => a.Id == boostedArtPieceId)
                        : null;
                ArtistProfileModel model = new()
                {
                        Id = artist.Id.Value,
                        Name = artist.Name,
                        Summary = artist.Summary,
                        IsOwner = artist.UserId == GetUserId(),
                        IsAdmin = await userManager.IsInRoleAsync(
                                (await userManager.FindByIdAsync(GetUserId().ToString()))!,
                                Constants.ADMIN_ROLE),
                        BoostedArtPiecePath = boostedArtPiece?.ImagePath,
                        BoostExpirationDate = artist.ActiveBoost?.ExpirationDate,
                };

                await missionManager.RecordProgressAsync(
                        MissionType.VisitArtistsProfiles,
                        GetUserId(),
                        DateTimeOffset.UtcNow);

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
        public async Task<ActionResult> Deactivate(Guid artistId)
        {
                if (await IsArtistAsync() == false)
                {
                        return RedirectToAction(nameof(Setup));
                }

                await deactivateArtistCommand.ExecuteAsync(GetUserId(), new ArtistId() { Value = artistId });
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

                return await userManager.IsInRoleAsync(user, Constants.ARTIST_ROLE);
        }

}
