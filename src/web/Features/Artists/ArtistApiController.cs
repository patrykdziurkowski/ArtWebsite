using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists.UpdateArtistProfile;

namespace web.Features.Artists;

[Authorize]
[ApiController]
public class ArtistApiController(ApplicationDbContext dbContext) : ControllerBase
{
        [HttpPut("/api/artist")]
        public async Task<IActionResult> UpdateArtistProfile(UpdateArtistProfileModel model)
        {
                Artist artist = await dbContext.Artists.FirstAsync(a => a.UserId == GetUserId());
                artist.Name = model.Name;
                artist.Summary = model.Summary;
                await dbContext.SaveChangesAsync();
                return Ok();
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
