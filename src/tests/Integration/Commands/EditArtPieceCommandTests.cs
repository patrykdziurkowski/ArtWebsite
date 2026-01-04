using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.ArtPieces;
using web.Features.ArtPieces.EditArtPiece;

namespace tests.Integration.Commands;

public class EditArtPieceCommandTests : DatabaseTest
{
        private readonly EditArtPieceCommand _command;

        public EditArtPieceCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<EditArtPieceCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenCurrentUserNotArtPieceOwnerOrAdmin()
        {
                await CreateUserWithArtistProfile("someUser", "someDiffArtist");
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceId = await DbContext.ArtPieces.Select(ap => ap.Id).FirstAsync();

                Func<Task> executingCommand = async () =>
                        await _command.ExecuteAsync(currentUserId, artPieceId, "Some new description");

                await executingCommand.Should().ThrowAsync<InvalidOperationException>();
                ArtPiece artPiece = await DbContext.ArtPieces.SingleAsync(ap => ap.Id == artPieceId);
                artPiece.Description.Should().NotBe("Some new description");
        }

        [Fact]
        public async Task ExecuteAsync_EditsArtPiece_WhenCurrentUserIsOwner()
        {
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                ArtPieceId artPieceId = await DbContext.ArtPieces.Select(ap => ap.Id).FirstAsync();

                await _command.ExecuteAsync(currentUserId, artPieceId, "Some new description");

                ArtPiece artPiece = await DbContext.ArtPieces.SingleAsync(ap => ap.Id == artPieceId);
                artPiece.Description.Should().Be("Some new description");
        }

        [Fact]
        public async Task ExecuteAsync_EditsArtPiece_WhenCurrentUserIsAdmin()
        {
                await CreateUserWithArtistProfile("someUser", "someDiffArtist");
                IdentityUser<Guid> currentUser = await DbContext.Users.SingleAsync();
                await UserManager.AddToRoleAsync(currentUser, Constants.ADMIN_ROLE);
                await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceId = await DbContext.ArtPieces.Select(ap => ap.Id).FirstAsync();

                await _command.ExecuteAsync(currentUser.Id, artPieceId, "Some new description");

                ArtPiece artPiece = await DbContext.ArtPieces.SingleAsync(ap => ap.Id == artPieceId);
                artPiece.Description.Should().Be("Some new description");
        }
}
