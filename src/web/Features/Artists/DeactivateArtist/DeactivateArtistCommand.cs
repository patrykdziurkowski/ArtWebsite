using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists.DeactivateArtist;

public class DeactivateArtistCommand(UserManager<IdentityUser<Guid>> userManager,
        ArtistRepository artistRepository,
        ApplicationDbContext dbContext)
{
        public async Task ExecuteAsync(Guid userId)
        {
                IdentityUser<Guid> user = await userManager.FindByIdAsync(userId.ToString())
                                              ?? throw new InvalidOperationException("Could not deactivate artist profile - user with such id does not exist.");
                Artist artist = await artistRepository.GetByUserIdAsync(userId)
                        ?? throw new InvalidOperationException("No artist exists for a given user.");
                dbContext.Artists.Remove(artist);
                await dbContext.SaveChangesAsync();
                await userManager.RemoveFromRoleAsync(user, "Artist");
        }
}
