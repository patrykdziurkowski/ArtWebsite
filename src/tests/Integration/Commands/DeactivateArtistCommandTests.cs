using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.Artists.DeactivateArtist;

namespace tests.Integration.Commands;

public class DeactivateArtistCommandTests : DatabaseBase
{
        private readonly DeactivateArtistCommand _command;

        public DeactivateArtistCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<DeactivateArtistCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenUserIdDoesntExist()
        {
                Func<Task> action = async () => await _command.ExecuteAsync(Guid.Empty);

                await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRemoveArtistEntityAndRole_WhenExists()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Artists.Add(new Artist()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();

                await _command.ExecuteAsync(user.Id);

                (await DbContext.Artists.FirstOrDefaultAsync(a => a.Name == "ArtistName"))
                        .Should().BeNull();
                (await UserManager.IsInRoleAsync(user, "Artist")).Should().BeFalse();
        }

}
