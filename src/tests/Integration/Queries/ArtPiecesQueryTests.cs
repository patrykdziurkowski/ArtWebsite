using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.LoadArtPieces;

namespace tests.Integration.Queries;

public class ArtPiecesQueryTests : DatabaseTest
{
        private readonly ArtPiecesQuery _command;

        public ArtPiecesQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<ArtPiecesQuery>();
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenNoArtPieces()
        {
                List<ArtPiece> artPieces = await _command.ExecuteAsync(new ArtistId(), 10);

                artPieces.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenArtPieceExistsButByADifferentArtist()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = await _command.ExecuteAsync(new ArtistId(), 10);

                artPieces.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturn6ArtPieces_WhenArtistHas6ArtPieces()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = await _command.ExecuteAsync(artistId, 10);

                artPieces.Should().HaveCount(6);
        }

        [Fact]
        public async Task Execute_ShouldReturn1ArtPiece_GivenOffsetWhenArtistHas6ArtPieces()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = await _command.ExecuteAsync(artistId, 10, 5);

                artPieces.Should().HaveCount(1);
        }

        [Fact]
        public async Task Execute_ShouldReturn3ArtPieces_WhenAskedFor3AndArtistHas6()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = await _command.ExecuteAsync(artistId, 3, 0);

                artPieces.Should().HaveCount(3);
        }

}
