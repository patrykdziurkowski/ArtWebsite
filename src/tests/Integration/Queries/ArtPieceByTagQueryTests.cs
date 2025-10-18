using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Browse.ByTag;

namespace tests.Integration.Queries;

public class ArtPieceByTagQueryTests : DatabaseTest
{
        private readonly ArtPieceByTagQuery _query;
        private readonly UploadArtPieceCommand _uploadArtPieceCommand;

        public ArtPieceByTagQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ArtPieceByTagQuery>();
                _uploadArtPieceCommand = Scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNull_WhenNoArtPiecesInDatabase()
        {
                await CreateUserWithArtistProfile();
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPieceDto? artPiece = await _query.ExecuteAsync(currentUserId, "someTag");

                artPiece.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNull_WhenArtPieceExistsButDoesntHaveTheGivenTag()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                Guid currentUserId = DbContext.Users.First().Id;
                await Create6ArtPiecesForArtist(artistId);
                await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", currentUserId);
                await WaitWhileNoTagsInDatabaseAsync();
                string tagName = await DbContext.Tags.Select(t => t.Name).FirstAsync() + "someInvalidTagName";

                ArtPieceDto? artPiece = await _query.ExecuteAsync(currentUserId, tagName);

                artPiece.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnArtPiece_WhenArtPieceHasTheGivenTag()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                Guid currentUserId = DbContext.Users.First().Id;
                await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", currentUserId);
                await WaitWhileNoTagsInDatabaseAsync();
                string tagName = await DbContext.Tags.Select(t => t.Name).FirstAsync();

                ArtPieceDto? artPiece = await _query.ExecuteAsync(currentUserId, tagName);

                artPiece.Should().NotBeNull();
        }
}
