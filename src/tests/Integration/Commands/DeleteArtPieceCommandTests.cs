
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.ArtPieces;
using web.Features.ArtPieces.DeleteArtPiece;

namespace tests.Integration.Commands;

public class DeleteArtPieceCommandTests : DatabaseTest
{
        private readonly DeleteArtPieceCommand _command;

        public DeleteArtPieceCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<DeleteArtPieceCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenCurrentUserIsNotArtPieceOwnerNorAdmin()
        {
                await CreateUserWithArtistProfile("someUser", "someArtist");
                Guid otherUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId someArtPieceId = (await CreateArtistUserWithArtPieces()).First();

                Func<Task> executing = async () => await _command.ExecuteAsync(otherUserId, someArtPieceId);

                await executing.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenArtPieceDoesntExist()
        {
                await CreateArtistUserWithArtPieces();
                Guid userId = (await DbContext.Users.SingleAsync()).Id;

                Func<Task> executing = async () => await _command.ExecuteAsync(userId, new ArtPieceId());

                await executing.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ExecuteAsync_DeletesArtPiece_WhenCurrentUserIsArtPieceOwner()
        {
                ArtPieceId someArtPieceId = (await CreateArtistUserWithArtPieces()).First();
                Guid userId = (await DbContext.Users.SingleAsync()).Id;
                int countBeforeDeletion = await DbContext.ArtPieces.CountAsync();

                await _command.ExecuteAsync(userId, someArtPieceId);

                int countAfterDeletion = await DbContext.ArtPieces.CountAsync();
                countAfterDeletion.Should().Be(countBeforeDeletion - 1);
                (await DbContext.ArtPieces.AnyAsync(ap => ap.Id == someArtPieceId)).Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_DeletesArtPiece_WhenCurrentUserIsNotArtPieceOwnerButAdmin()
        {
                IdentityUser<Guid> admin = new("adminAdmin");
                await UserManager.CreateAsync(admin);
                await UserManager.AddToRoleAsync(admin, Constants.ADMIN_ROLE);

                ArtPieceId someArtPieceId = (await CreateArtistUserWithArtPieces()).First();
                int countBeforeDeletion = await DbContext.ArtPieces.CountAsync();

                await _command.ExecuteAsync(admin.Id, someArtPieceId);

                int countAfterDeletion = await DbContext.ArtPieces.CountAsync();
                countAfterDeletion.Should().Be(countBeforeDeletion - 1);
                (await DbContext.ArtPieces.AnyAsync(ap => ap.Id == someArtPieceId)).Should().BeFalse();
        }
}
