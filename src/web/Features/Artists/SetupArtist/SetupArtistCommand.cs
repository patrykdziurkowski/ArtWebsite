using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace web.Features.Artists.SetupArtist;

public class SetupArtistCommand(ArtistRepository artistRepository,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task<Result<Artist>> ExecuteAsync(Guid currentUserId, string name,
                string summary)
        {
                IdentityUser<Guid> user = await userManager.FindByIdAsync(currentUserId.ToString())
                        ?? throw new InvalidOperationException("Could not setup artist profile - user with such id does not exist.");


                if (await artistRepository.GetByNameAsync(name) is not null)
                {
                        return Result.Fail($"An artist with name '{name}' already exists.");
                }

                if (await artistRepository.GetByUserIdAsync(currentUserId) is not null)
                {
                        throw new InvalidOperationException("This user already has an artist profile.");
                }

                Artist artist = new()
                {
                        UserId = currentUserId,
                        Name = name,
                        Summary = summary,
                };
                await artistRepository.SaveChangesAsync(artist);
                await userManager.AddToRoleAsync(user, Constants.ARTIST_ROLE);
                return Result.Ok();
        }
}
