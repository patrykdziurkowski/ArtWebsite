using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.Artists;
using web.Features.Artists.DeactivateArtist;

namespace tests.Integration.Commands;

public class DeactivateArtistCommandTests : DatabaseTest
{
        private readonly DeactivateArtistCommand _command;

        public DeactivateArtistCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<DeactivateArtistCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenUserDoesntExist()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                DbContext.Artists.Add(artist);
                await DbContext.SaveChangesAsync();

                Func<Task> action = async () => await _command.ExecuteAsync(Guid.NewGuid(), artist.Id);

                await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenArtistDoesntExist()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                DbContext.Artists.Add(artist);
                await DbContext.SaveChangesAsync();

                Func<Task> action = async () => await _command.ExecuteAsync(user.Id, new ArtistId());

                await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenCurrentUserNotArtistNorAdmin()
        {
                IdentityUser<Guid> notAdmin = new("adminAdmin");
                await UserManager.CreateAsync(notAdmin);

                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                DbContext.Artists.Add(artist);
                await DbContext.SaveChangesAsync();

                Func<Task> action = async () => await _command.ExecuteAsync(notAdmin.Id, artist.Id);

                await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRemoveArtistEntityAndRoleFromOwner_WhenCurrentUserNotArtistButIsAdmin()
        {
                IdentityUser<Guid> admin = new("adminAdmin");
                await UserManager.CreateAsync(admin);
                await UserManager.AddToRoleAsync(admin, Constants.ADMIN_ROLE);

                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                DbContext.Artists.Add(artist);
                await DbContext.SaveChangesAsync();

                await _command.ExecuteAsync(admin.Id, artist.Id);

                (await DbContext.Artists.FirstOrDefaultAsync(a => a.Name == "ArtistName"))
                        .Should().BeNull();
                (await UserManager.IsInRoleAsync(user, Constants.ARTIST_ROLE)).Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRemoveArtistEntityAndRole_WhenExists()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                DbContext.Artists.Add(artist);
                await DbContext.SaveChangesAsync();

                await _command.ExecuteAsync(user.Id, artist.Id);

                (await DbContext.Artists.FirstOrDefaultAsync(a => a.Name == "ArtistName"))
                        .Should().BeNull();
                (await UserManager.IsInRoleAsync(user, Constants.ARTIST_ROLE)).Should().BeFalse();
        }

}
