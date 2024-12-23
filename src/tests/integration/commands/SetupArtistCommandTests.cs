using FluentAssertions;
using FluentResults;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public SetupArtistCommandTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<SetupArtistCommand>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _dbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                _dbContext.Database.RollbackTransaction();
                _scope.Dispose();
        }

        [Fact]
        public async Task Execute_ShouldFail_WhenNameAlreadyTaken()
        {
                _dbContext.Artists.Add(
                        new Artist(new ArtistId(), "ArtistName", "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();

                Result<Artist> result = await _command.Execute("ArtistName", "Some other summary for some other artist.");

                result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_ShouldSaveArtist_WhenNameNotTaken()
        {
                Result<Artist> result = await _command.Execute("ArtistName", "Some other summary for some other artist.");

                Artist artist = await _dbContext.Artists.FirstAsync();
                artist.Name.Should().Be("ArtistName");
                artist.Summary.Should().Be("Some other summary for some other artist.");
                result.IsSuccess.Should().BeTrue();
        }
}
