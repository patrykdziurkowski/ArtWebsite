using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.Artists;
using web.Features.Artists.SetupArtist;

namespace tests.Integration.Commands;

public class SetupArtistCommandTests : DatabaseTest
{
        private readonly SetupArtistCommand _command;

        public SetupArtistCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<SetupArtistCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenNameAlreadyTaken()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Artists.Add(new Artist
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();

                Result<Artist> result = await _command.ExecuteAsync(user.Id,
                        "ArtistName", "Some other summary for some other artist.");

                result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSaveArtistAndAddArtistRole_WhenNameNotTaken()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);

                Result<Artist> result = await _command.ExecuteAsync(user.Id,
                        "ArtistName", "Some other summary for some other artist.");

                Artist artist = await DbContext.Artists.FirstAsync();
                artist.Name.Should().Be("ArtistName");
                artist.Summary.Should().Be("Some other summary for some other artist.");
                result.IsSuccess.Should().BeTrue();
                (await UserManager.IsInRoleAsync(user, Constants.ARTIST_ROLE)).Should().BeTrue();
        }
}
