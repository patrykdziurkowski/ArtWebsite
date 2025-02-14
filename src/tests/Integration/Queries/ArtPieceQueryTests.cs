using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.Index;
using web.Features.Reviews;

namespace tests.Integration.Queries;

public class ArtPieceQueryTests : DatabaseBase
{
        private readonly ArtPieceQuery _command;

        public ArtPieceQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<ArtPieceQuery>();
        }

        [Fact]
        public void Execute_ShouldReturnNull_WhenNoArtPiecesInDatabase()
        {
                ArtPiece? artPiece = _command.Execute(Guid.NewGuid());

                artPiece.Should().BeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnAnArtPiece_WhenOneExists()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                ArtPiece? artPiece = _command.Execute(Guid.NewGuid());

                artPiece.Should().NotBeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnNull_WhenOneExistsButWasReviewed()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                ArtPiece artPiece = new($"somePath1", "description", artistId);
                await DbContext.ArtPieces.AddAsync(artPiece);
                await DbContext.Reviews.AddAsync(new Review
                {
                        Comment = "Some review comment!",
                        ArtPieceId = artPiece.Id,
                        ReviewerId = DbContext.Users.First().Id,
                });
                await DbContext.SaveChangesAsync();

                ArtPiece? returnedArtPiece = _command.Execute(artistId.Value);

                returnedArtPiece.Should().NotBeNull();
        }

}
