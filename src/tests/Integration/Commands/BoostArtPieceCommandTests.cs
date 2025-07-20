using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.Artists.BoostArtPiece;
using web.Features.ArtPieces;

namespace tests.Integration.Commands;

public class BoostArtPieceCommandTests : DatabaseBase
{
        private readonly BoostArtPieceCommand _command;
        public BoostArtPieceCommandTests(DatabaseTestContext context) : base(context)
        {
                _command = Scope.ServiceProvider.GetRequiredService<BoostArtPieceCommand>();
        }

        [Fact]
        public async Task Execute_ShouldThrow_WhenUserHasNoArtistProfile()
        {
                IdentityUser<Guid> user = new("johnsmith");
                await UserManager.CreateAsync(user);
                Guid userId = DbContext.Users.First().Id;

                Func<Task> method = async () => await _command.ExecuteAsync(userId, new ArtPieceId());

                await method.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrow_WhenArtPieceDoesntExist()
        {
                await CreateUserWithArtistProfile();
                Guid userId = DbContext.Users.First().Id;

                Func<Task> method = async () => await _command.ExecuteAsync(userId, new ArtPieceId());

                await method.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Execute_ShouldReturnFail_WhenArtPieceDoesntBelongToCurrentArtist()
        {
                await CreateUserWithArtistProfile("artistBoosting");
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First(u => u.UserName == "artistBoosting").Id;
                ArtPieceId artPieceId = DbContext.ArtPieces.First().Id;

                Result<BoostDto> result = await _command.ExecuteAsync(currentUserId, artPieceId);

                result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_ShouldReturnFail_WhenArtPieceAlreadyBoosted()
        {
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                ArtPieceId artPieceId = DbContext.ArtPieces.First().Id;
                Result<BoostDto> firstResult = await _command.ExecuteAsync(currentUserId, artPieceId);

                Result<BoostDto> secondResult = await _command.ExecuteAsync(currentUserId, artPieceId);

                firstResult.IsSuccess.Should().BeTrue();
                secondResult.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_ShouldReturnBoost_WhenArtPieceSuccessfullyBoosted()
        {
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                ArtPieceId artPieceId = DbContext.ArtPieces.First().Id;

                Result<BoostDto> result = await _command.ExecuteAsync(currentUserId, artPieceId);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
        }
}
