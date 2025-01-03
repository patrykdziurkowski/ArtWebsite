using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.integration.fixtures;
using web.data;
using web.features.artist;
using web.features.artist.SetupArtist;

namespace tests.integration.commands;

[Collection("Database collection")]
public class SetupArtistCommandTests : IDisposable
{
        private readonly SetupArtistCommand _command;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public SetupArtistCommandTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<SetupArtistCommand>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
                _dbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                _dbContext.Database.RollbackTransaction();
                _scope.Dispose();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenNameAlreadyTaken()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);
                _dbContext.Artists.Add(
                        new Artist(user.Id, "ArtistName",
                        "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();

                Result<Artist> result = await _command.ExecuteAsync(user.Id,
                        "ArtistName", "Some other summary for some other artist.");

                result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSaveArtistAndAddArtistRole_WhenNameNotTaken()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);

                Result<Artist> result = await _command.ExecuteAsync(user.Id,
                        "ArtistName", "Some other summary for some other artist.");

                Artist artist = await _dbContext.Artists.FirstAsync();
                artist.Name.Should().Be("ArtistName");
                artist.Summary.Should().Be("Some other summary for some other artist.");
                result.IsSuccess.Should().BeTrue();
                (await _userManager.IsInRoleAsync(user, "Artist")).Should().BeTrue();
        }
}
