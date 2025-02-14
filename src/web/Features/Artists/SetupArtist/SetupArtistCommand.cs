using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists.SetupArtist;

public class SetupArtistCommand
{
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public SetupArtistCommand(ApplicationDbContext dbContext,
                UserManager<IdentityUser<Guid>> userManager)
        {
                _dbContext = dbContext;
                _userManager = userManager;
        }

        public async Task<Result<Artist>> ExecuteAsync(Guid userId, string name, string summary)
        {
                IdentityUser<Guid> user = await _userManager.FindByIdAsync(userId.ToString())
                        ?? throw new InvalidOperationException("Could not setup artist profile - user with such id does not exist.");

                if (await _dbContext.Artists.AnyAsync(a => a.Name == name))
                {
                        return Result.Fail($"An artist with name '{name}' already exists.");
                }

                _dbContext.Add(new Artist
                {
                        UserId = userId,
                        Name = name,
                        Summary = summary,
                });
                await _dbContext.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, "Artist");
                return Result.Ok();
        }
}
