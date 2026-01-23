using FluentResults;
using Microsoft.AspNetCore.Identity;
using web.Features.Images;

namespace web.Features.Artists.UpdateArtistProfile;

public class UpdateArtistCommand(
        ArtistRepository artistRepository,
        UserManager<IdentityUser<Guid>> userManager,
        ImageManager imageManager)
{
        public async Task<Result> ExecuteAsync(
                Guid currentUserId,
                ArtistId artistId,
                string name,
                string summary,
                IFormFile? newProfilePicture = null)
        {
                Artist? artist = await artistRepository.GetByIdAsync(artistId);
                if (artist is null)
                {
                        return Result.Fail("No artist with such id exists.");
                }

                if (artist.UserId != currentUserId)
                {
                        IdentityUser<Guid>? currentUser = await userManager.FindByIdAsync(currentUserId.ToString());
                        bool isAdmin = await userManager.IsInRoleAsync(currentUser!, Constants.ADMIN_ROLE);
                        if (!isAdmin)
                        {
                                return Result.Fail("Artist profile does not belong to the current user.");
                        }
                }

                artist.Name = name;
                artist.Summary = summary;
                if (newProfilePicture is not null)
                {
                        artist.ProfilePicturePath = await imageManager.UpdateArtistProfilePictureAsync(
                                newProfilePicture, artistId);
                }

                await artistRepository.SaveChangesAsync(artist);
                return Result.Ok();
        }
}
