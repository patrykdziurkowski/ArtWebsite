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
        public async Task Execute_ShouldReturnNull_WhenNoArtPiecesInDatabase()
        {
                await CreateUserWithArtistProfile();
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPiece? artPiece = _command.Execute(currentUserId);

                artPiece.Should().BeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnAnArtPiece_WhenOneExists()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPiece? artPiece = _command.Execute(currentUserId);

                artPiece.Should().NotBeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnNull_WhenOneExistsButWasReviewed()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                ArtPiece artPiece = new()
                {
                        ImagePath = "somePath1",
                        Description = "description",
                        ArtistId = artistId,
                };
                await DbContext.ArtPieces.AddAsync(artPiece);
                await DbContext.Reviews.AddAsync(new Review
                {
                        Comment = "Some review comment!",
                        ArtPieceId = artPiece.Id,
                        ReviewerId = DbContext.Reviewers.First().Id,
                });
                await DbContext.SaveChangesAsync();
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPiece? returnedArtPiece = _command.Execute(currentUserId);

                returnedArtPiece.Should().BeNull();
        }

}
