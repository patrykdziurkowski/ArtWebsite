using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.Artists;
using web.Features.Artists.UpdateArtistProfile;
using web.Features.Reviewers;

namespace tests.Integration.Commands;

public class UpdateArtistCommandTests : DatabaseTest
{
        private readonly UpdateArtistCommand _command;

        public UpdateArtistCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<UpdateArtistCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_IfArtistDoesntExist()
        {
                IdentityUser<Guid> user = new("SomeUser");
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                await DbContext.SaveChangesAsync();

                Result result = await _command.ExecuteAsync(user.Id, new ArtistId(), "Name123", "Summary.");

                result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_IfArtistIsntCurrentUserNorAdmin()
        {
                IdentityUser<Guid> user = new("SomeUser");
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                await DbContext.SaveChangesAsync();

                await CreateUserWithArtistProfile();

                Result result = await _command.ExecuteAsync(
                        user.Id,
                        (await DbContext.Artists.SingleAsync()).Id,
                        "Name123",
                        "Summary.");

                result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSucceed_IfArtistIsCurrentUser()
        {
                await CreateUserWithArtistProfile();

                Result result = await _command.ExecuteAsync(
                        (await DbContext.Users.SingleAsync()).Id,
                        (await DbContext.Artists.SingleAsync()).Id,
                        "Name123",
                        "Summary.");

                result.IsFailed.Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSucceed_IfArtistIsntCurrentUserButIsAdmin()
        {
                IdentityUser<Guid> user = new("SomeUser");
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                await DbContext.SaveChangesAsync();
                await UserManager.AddToRoleAsync(user, Constants.ADMIN_ROLE);

                await CreateUserWithArtistProfile();

                Result result = await _command.ExecuteAsync(
                        user.Id,
                        (await DbContext.Artists.SingleAsync()).Id,
                        "Name123",
                        "Summary.");

                result.IsFailed.Should().BeFalse();
        }
}
