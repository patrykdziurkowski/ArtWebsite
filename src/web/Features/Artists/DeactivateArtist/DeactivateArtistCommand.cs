using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists.DeactivateArtist;

public class DeactivateArtistCommand(
        UserManager<IdentityUser<Guid>> userManager,
        ArtistRepository artistRepository,
        ApplicationDbContext dbContext)
{
        public async Task ExecuteAsync(Guid currentUserId, ArtistId artistId)
        {
                IdentityUser<Guid> currentUser = await userManager.FindByIdAsync(currentUserId.ToString())
                                              ?? throw new InvalidOperationException("Could not deactivate artist profile - current user with such id does not exist.");
                Artist artist = await artistRepository.GetByIdAsync(artistId)
                        ?? throw new InvalidOperationException("No artist exists for a given user.");
                if (artist.UserId != currentUserId
                        && !await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE))
                {
                        throw new InvalidOperationException("Attempted to deactivate an artist profile that does not belong to the given user.");
                }

                IdentityUser<Guid> artistOwner = await userManager.FindByIdAsync(artist.UserId.ToString())
                                              ?? throw new InvalidOperationException("Could not deactivate artist profile - no user owns this artist's profile.");
                await dbContext.ArtistPointAwards
                        .Where(award => award.ArtistId == artist.Id)
                        .ExecuteDeleteAsync();
                await dbContext.ArtPiecesServed
                        .Where(aps => aps.UserId == artist.UserId)
                        .ExecuteDeleteAsync();
                artist.Deactivate();
                await userManager.RemoveFromRoleAsync(artistOwner, Constants.ARTIST_ROLE);
                await artistRepository.SaveChangesAsync(artist);
        }
}
