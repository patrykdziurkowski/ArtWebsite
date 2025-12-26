using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Browse;

namespace tests.Integration.Commands;

public class RegisterArtPieceServedCommandTests : DatabaseTest
{
        private readonly RegisterArtPieceServedCommand _command;

        public RegisterArtPieceServedCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<RegisterArtPieceServedCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldAddNew_WhenNotPreviouslyPresent()
        {
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId artPieceId = (await DbContext.ArtPieces.FirstAsync()).Id;

                ArtPieceServed? artPieceServedBefore = await DbContext.ArtPiecesServed.SingleOrDefaultAsync();

                await _command.ExecuteAsync(currentUserId, artPieceId);

                artPieceServedBefore.Should().BeNull();
                ArtPieceServed artPieceServedAfter = await DbContext.ArtPiecesServed.SingleAsync();
                artPieceServedAfter.UserId.Should().Be(currentUserId);
                artPieceServedAfter.ArtPieceId.Should().Be(artPieceId);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateExisting_WhenPreviouslyPresent()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId artPieceId1 = artPieceIds[0];
                ArtPieceId artPieceId2 = artPieceIds[1];

                await _command.ExecuteAsync(currentUserId, artPieceId1, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(12)));
                ArtPieceServed servedArtPiece = await DbContext.ArtPiecesServed.SingleAsync();
                DateTimeOffset dateTimeOffsetBefore = servedArtPiece.Date;
                Guid userIdBefore = servedArtPiece.UserId;
                ArtPieceId artPieceIdBefore = servedArtPiece.ArtPieceId;
                ArtPieceServedId artPieceServedIdBefore = servedArtPiece.Id;

                await _command.ExecuteAsync(currentUserId, artPieceId2, DateTimeOffset.UtcNow);
                servedArtPiece = await DbContext.ArtPiecesServed.SingleAsync();

                servedArtPiece.UserId.Should().Be(currentUserId);
                servedArtPiece.UserId.Should().Be(userIdBefore);
                servedArtPiece.ArtPieceId.Should().Be(artPieceId2);
                artPieceIdBefore.Should().Be(artPieceId1);
                servedArtPiece.Id.Should().Be(artPieceServedIdBefore);
                servedArtPiece.Date.Should().NotBe(dateTimeOffsetBefore);

        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateExisting_WhenPreviouslyPresentEvenWithTheSameArtPieceId()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId artPieceId = artPieceIds.First();

                await _command.ExecuteAsync(currentUserId, artPieceId, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(12)));
                ArtPieceServed servedArtPiece = await DbContext.ArtPiecesServed.SingleAsync();
                DateTimeOffset dateTimeOffsetBefore = servedArtPiece.Date;
                Guid userIdBefore = servedArtPiece.UserId;
                ArtPieceId artPieceIdBefore = servedArtPiece.ArtPieceId;
                ArtPieceServedId artPieceServedIdBefore = servedArtPiece.Id;

                await _command.ExecuteAsync(currentUserId, artPieceId, DateTimeOffset.UtcNow);
                servedArtPiece = await DbContext.ArtPiecesServed.SingleAsync();

                servedArtPiece.UserId.Should().Be(currentUserId);
                userIdBefore.Should().Be(currentUserId);
                servedArtPiece.ArtPieceId.Should().Be(artPieceId);
                artPieceIdBefore.Should().Be(artPieceId);
                servedArtPiece.Id.Should().Be(artPieceServedIdBefore);
                servedArtPiece.Date.Should().NotBe(dateTimeOffsetBefore);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCreateNew_EvenIfAlreadyExistsForDifferentUser()
        {
                await CreateArtistUserWithArtPieces();
                await CreateArtistUserWithArtPieces("johnSmith2", "reviewer2", "artist2");
                List<IdentityUser<Guid>> users = await DbContext.Users.ToListAsync();
                Guid currentUserId1 = users[0].Id;
                Guid currentUserId2 = users[1].Id;
                ArtPieceId artPieceId = (await DbContext.ArtPieces.FirstAsync()).Id;

                await _command.ExecuteAsync(currentUserId1, artPieceId);
                await _command.ExecuteAsync(currentUserId2, artPieceId);

                List<ArtPieceServed> artPiecesServed = await DbContext.ArtPiecesServed.ToListAsync();
                artPiecesServed.Should().HaveCount(2);
                artPiecesServed[0].Id.Should().NotBe(artPiecesServed[1].Id);
                artPiecesServed[0].Date.Should().NotBe(artPiecesServed[1].Date);
                artPiecesServed[0].ArtPieceId.Should().Be(artPiecesServed[1].ArtPieceId);
                artPiecesServed[0].UserId.Should().NotBe(artPiecesServed[1].UserId);
        }
}
