using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists.DeactivateArtist;

public class DeactivateArtistCommand
{
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public DeactivateArtistCommand(UserManager<IdentityUser<Guid>> userManager,
                ApplicationDbContext dbContext)
        {
                _userManager = userManager;
                _dbContext = dbContext;
        }

        public async Task ExecuteAsync(Guid userId)
        {
                IdentityUser<Guid> user = await _userManager.FindByIdAsync(userId.ToString())
                                              ?? throw new InvalidOperationException("Could not deactivate artist profile - user with such id does not exist.");
                Artist artist = await _dbContext.Artists.FirstAsync(a => a.UserId == user.Id);
                _dbContext.Artists.Remove(artist);
                await _dbContext.SaveChangesAsync();
                await _userManager.RemoveFromRoleAsync(user, "Artist");
        }
}
