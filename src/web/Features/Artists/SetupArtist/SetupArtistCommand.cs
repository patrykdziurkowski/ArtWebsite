using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace web.Features.Artists.SetupArtist;

public class SetupArtistCommand(ArtistRepository artistRepository,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task<Result<Artist>> ExecuteAsync(Guid userId, string name,
                string summary)
        {
                IdentityUser<Guid> user = await userManager.FindByIdAsync(userId.ToString())
                        ?? throw new InvalidOperationException("Could not setup artist profile - user with such id does not exist.");


                if (await artistRepository.GetByNameAsync(name) is not null)
                {
                        return Result.Fail($"An artist with name '{name}' already exists.");
                }

                Artist artist = new()
                {
                        UserId = userId,
                        Name = name,
                        Summary = summary,
                };
                await artistRepository.SaveChangesAsync(artist);
                await userManager.AddToRoleAsync(user, "Artist");
                return Result.Ok();
        }
}
