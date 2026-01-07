using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.Suspensions;

namespace tests.Integration.Commands;

public class SuspendUserCommandTests : DatabaseTest
{
        private readonly SuspendUserCommand _command;

        public SuspendUserCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<SuspendUserCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_IfCurrentUserIsNotAdmin()
        {
                await CreateUserWithArtistProfile();
                IdentityUser<Guid> currentUser = await DbContext.Users.SingleAsync();

                await CreateUserWithArtistProfile("targetUser", "targetArtist");
                IdentityUser<Guid> userToSuspend = await DbContext.Users.SingleAsync(u => u.Id != currentUser.Id);

                Func<Task> executingCommand = async () =>
                        await _command.ExecuteAsync(
                                currentUser.Id,
                                userToSuspend.Id,
                                TimeSpan.FromDays(7),
                                "Some reason.");

                await executingCommand.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_IfTryingToSuspendAnAdmin()
        {
                await CreateUserWithArtistProfile();
                IdentityUser<Guid> currentUser = await DbContext.Users.SingleAsync();
                await UserManager.AddToRoleAsync(currentUser, Constants.ADMIN_ROLE);

                await CreateUserWithArtistProfile("targetUser", "targetArtist");
                IdentityUser<Guid> userToSuspend = await DbContext.Users.SingleAsync(u => u.Id != currentUser.Id);
                await UserManager.AddToRoleAsync(userToSuspend, Constants.ADMIN_ROLE);

                Func<Task> executingCommand = async () =>
                        await _command.ExecuteAsync(
                                currentUser.Id,
                                userToSuspend.Id,
                                TimeSpan.FromDays(7),
                                "Some reason.");

                await executingCommand.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_CreateSuspensionRecord_WhenSuccessful()
        {
                DateTimeOffset now = new DateTime(1997, 2, 18);

                await CreateUserWithArtistProfile();
                IdentityUser<Guid> currentUser = await DbContext.Users.SingleAsync();
                await UserManager.AddToRoleAsync(currentUser, Constants.ADMIN_ROLE);

                await CreateUserWithArtistProfile("targetUser", "targetArtist");
                IdentityUser<Guid> userToSuspend = await DbContext.Users.SingleAsync(u => u.Id != currentUser.Id);

                Suspension suspension = await _command.ExecuteAsync(
                        currentUser.Id,
                        userToSuspend.Id,
                        TimeSpan.FromDays(7),
                        "Some reason",
                        now);

                DateTimeOffset expectedExpiry = new DateTime(1997, 2, 25);
                suspension.ExpiryDate.Should().Be(expectedExpiry);
                suspension.IssuedAt.Should().Be(now);
                suspension.UserId.Should().Be(userToSuspend.Id);
                suspension.IssuingUserId.Should().Be(currentUser.Id);
        }
}
