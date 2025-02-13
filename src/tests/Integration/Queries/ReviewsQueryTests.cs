using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviews;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Queries;

[Collection("Database collection")]
public class ReviewsQueryTests : IDisposable
{
        private readonly ReviewsQuery _command;
        private readonly ReviewArtPieceCommand _reviewArtPieceCommand;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public ReviewsQueryTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<ReviewsQuery>();
                _reviewArtPieceCommand = _scope.ServiceProvider.GetRequiredService<ReviewArtPieceCommand>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
                _dbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                _dbContext.Database.RollbackTransaction();
                _scope.Dispose();
        }

        [Fact]
        public void Execute_ShouldReturnEmpty_WhenNoReviewsForGivenUser()
        {
                List<Review> reviews = _command.Execute(Guid.NewGuid(), 10);

                reviews.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturnReviews_WhenTheyExist()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid reviewerId = await CreateUserWith20Reviews(artPieceIds);

                List<Review> reviews = _command.Execute(reviewerId, 10);

                reviews.Should().HaveCount(10);
        }

        [Fact]
        public async Task Execute_ShouldReturnSomeReviews_WhenOffset()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid reviewerId = await CreateUserWith20Reviews(artPieceIds);

                List<Review> reviews = _command.Execute(reviewerId, 10, 17);

                reviews.Should().HaveCount(3);
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenReviewsExistButForADifferentUser()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                await CreateUserWith20Reviews(artPieceIds);

                List<Review> reviews = _command.Execute(Guid.NewGuid(), 10, 0);

                reviews.Should().BeEmpty();
        }

        private async Task<List<ArtPieceId>> CreateArtistUserWithArtPieces()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);
                ArtistId artistId = new();
                _dbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                List<ArtPieceId> artPieceIds = [];
                for (int i = 0; i < 20; ++i)
                {
                        ArtPiece artPiece = new($"somePath", "description", artistId);
                        await _dbContext.ArtPieces.AddAsync(artPiece);
                        artPieceIds.Add(artPiece.Id);
                }
                await _dbContext.SaveChangesAsync();
                return artPieceIds;
        }

        private async Task<Guid> CreateUserWith20Reviews(List<ArtPieceId> artPiecesToReview)
        {
                IdentityUser<Guid> user = new("johnSmith2");
                await _userManager.CreateAsync(user);
                foreach (ArtPieceId artPieceId in artPiecesToReview)
                {
                        await _reviewArtPieceCommand.ExecuteAsync("Some comment with a descriptive opinion that's long enough",
                                artPieceId, user.Id);
                }
                await _dbContext.SaveChangesAsync();
                return user.Id;
        }

}
