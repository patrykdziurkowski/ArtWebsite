using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.data;

namespace web.features.artist.SetupArtist;

public class SetupArtistCommand
{
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public SetupArtistCommand(ApplicationDbContext dbContext,
                UserManager<IdentityUser> userManager)
        {
                _dbContext = dbContext;
                _userManager = userManager;
        }

        public async Task<Result<Artist>> ExecuteAsync(string ownerId, string name, string summary)
        {
                IdentityUser user = await _userManager.FindByIdAsync(ownerId)
                        ?? throw new InvalidOperationException("Could not setup artist profile - user with such id does not exist.");

                if (await _dbContext.Artists.AnyAsync(a => a.Name == name))
                {
                        return Result.Fail($"An artist with name '{name}' already exists.");
                }

                _dbContext.Add(new Artist(ownerId, name, summary));
                await _dbContext.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, "Artist");
                return Result.Ok();
        }
}
