using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;

namespace tests.Integration.Queries;

public class ArtPieceDetailsQueryTests : DatabaseTest
{
        private readonly ArtPieceDetailsQuery _query;

        public ArtPieceDetailsQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ArtPieceDetailsQuery>();
        }

        [Fact]
        public async Task ExecuteAsync_()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();

                ArtPieceDto artPieceDto = await _query.ExecuteAsync(artPieceIds.First());

                artPieceDto.ArtistName.Should().Be("ArtistName");
                artPieceDto.ImagePath.Should().NotBeNullOrEmpty();
        }
}
