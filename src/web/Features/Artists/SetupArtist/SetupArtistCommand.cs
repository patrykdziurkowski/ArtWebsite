using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists.SetupArtist;

public class SetupArtistCommand(ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task<Result<Artist>> ExecuteAsync(Guid userId, string name,
                string summary)
        {
                IdentityUser<Guid> user = await userManager.FindByIdAsync(userId.ToString())
                        ?? throw new InvalidOperationException("Could not setup artist profile - user with such id does not exist.");

                if (await dbContext.Artists.AnyAsync(a => a.Name == name))
                {
                        return Result.Fail($"An artist with name '{name}' already exists.");
                }

                dbContext.Add(new Artist
                {
                        UserId = userId,
                        Name = name,
                        Summary = summary,
                });
                await dbContext.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "Artist");
                return Result.Ok();
        }
}
