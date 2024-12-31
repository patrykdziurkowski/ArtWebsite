using System;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.data;

namespace web.features.artist.DeactivateArtist;

public class DeactivateArtistCommand
{
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public DeactivateArtistCommand(UserManager<IdentityUser> userManager,
                ApplicationDbContext dbContext)
        {
                _userManager = userManager;
                _dbContext = dbContext;
        }

        public async Task ExecuteAsync(string userId)
        {
                IdentityUser user = await _userManager.FindByIdAsync(userId)
                                              ?? throw new InvalidOperationException("Could not deactivate artist profile - user with such id does not exist.");
                Artist artist = await _dbContext.Artists.FirstAsync(a => a.OwnerId == user.Id);
                _dbContext.Artists.Remove(artist);
                await _dbContext.SaveChangesAsync();
                await _userManager.RemoveFromRoleAsync(user, "Artist");
        }
}
