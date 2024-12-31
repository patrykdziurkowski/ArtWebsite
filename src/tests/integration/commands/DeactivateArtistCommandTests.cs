using System;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.integration.fixtures;
using web.data;
using web.features.artist;
using web.features.artist.DeactivateArtist;

namespace tests.integration.commands;

[Collection("Database collection")]
public class DeactivateArtistCommandTests : IDisposable
{
        private readonly DeactivateArtistCommand _command;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public DeactivateArtistCommandTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<DeactivateArtistCommand>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                _dbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                _dbContext.Database.RollbackTransaction();
                _scope.Dispose();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenUserIdDoesntExist()
        {
                Func<Task> action = async () => await _command.ExecuteAsync("invalidId");

                await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRemoveArtistEntityAndRole_WhenExists()
        {
                IdentityUser user = new("johnSmith");
                await _userManager.CreateAsync(user);
                _dbContext.Artists.Add(
                        new Artist(new ArtistId(), user.Id, "ArtistName", "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();

                await _command.ExecuteAsync(user.Id);

                (await _dbContext.Artists.FirstOrDefaultAsync(a => a.Name == "ArtistName")).Should().BeNull();
                (await _userManager.IsInRoleAsync(user, "Artist")).Should().BeFalse();
        }

}
