using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Browse;
using web.Features.Browse.Index;
using web.Features.Reviews;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Queries;

public class ArtPieceQueryTests : DatabaseTest
{
        private readonly ArtPieceQuery _query;

        private readonly RegisterArtPieceServedCommand _registerArtPieceServedCommand;
        private readonly ReviewArtPieceCommand _reviewArtPieceCommand;

        public ArtPieceQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ArtPieceQuery>();
                _registerArtPieceServedCommand = Scope.ServiceProvider.GetRequiredService<RegisterArtPieceServedCommand>();
                _reviewArtPieceCommand = Scope.ServiceProvider.GetRequiredService<ReviewArtPieceCommand>();
        }

        [Fact]
        public async Task Execute_ShouldReturnNull_WhenNoArtPiecesInDatabase()
        {
                await CreateUserWithArtistProfile();
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPieceDto? artPiece = await _query.ExecuteAsync(currentUserId);

                artPiece.Should().BeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnAnArtPiece_WhenOneExists()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await CreateArtPiecesForArtist(artistId);
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPieceDto? artPiece = await _query.ExecuteAsync(currentUserId);

                artPiece.Should().NotBeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnTheSameArtPiece_UntilItsReviewed()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await CreateArtPiecesForArtist(artistId);
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPieceId artPiece1 = (await _query.ExecuteAsync(currentUserId))!.Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPiece1);
                ArtPieceId artPiece2 = (await _query.ExecuteAsync(currentUserId))!.Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPiece2);
                ArtPieceId artPiece3 = (await _query.ExecuteAsync(currentUserId))!.Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPiece3);
                await ReviewArtPiece(currentUserId, artPiece3);
                ArtPieceId artPiece4 = (await _query.ExecuteAsync(currentUserId))!.Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPiece4);

                artPiece1.Should().Be(artPiece2);
                artPiece2.Should().Be(artPiece3);
                artPiece3.Should().NotBe(artPiece4);
        }

        [Fact]
        public async Task Execute_ShouldNotReturnASkippedArtPiece()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await CreateArtPiecesForArtist(artistId);
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPieceId artPiece1 = (await _query.ExecuteAsync(currentUserId))!.Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPiece1);
                DbContext.ArtPiecesServed.Single().WasSkipped = true;
                await DbContext.SaveChangesAsync();

                for (int i = 0; i < 100; i++)
                {
                        ArtPieceId returnedArtPieceId = (await _query
                                .ExecuteAsync(currentUserId))!.Id;
                        returnedArtPieceId.Should().NotBe(artPiece1);
                }
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
                        Rating = new Rating(5),
                        ReviewerId = DbContext.Reviewers.First().Id,
                });
                await DbContext.SaveChangesAsync();
                Guid currentUserId = DbContext.Users.First().Id;

                ArtPieceDto? returnedArtPiece = await _query.ExecuteAsync(currentUserId);

                returnedArtPiece.Should().BeNull();
        }

        private async Task ReviewArtPiece(Guid currentUserId, ArtPieceId artPieceId)
        {
                await _reviewArtPieceCommand.ExecuteAsync(
                        "Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. Some comment. ",
                        1,
                        artPieceId,
                        currentUserId,
                        reviewCooldown: TimeSpan.Zero);
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, null);
        }

}
