using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviews.LoadReviews;

namespace tests.Integration.Queries;

public class ReviewsQueryTests : DatabaseTest
{
        private readonly ReviewsQuery _command;

        public ReviewsQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<ReviewsQuery>();
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenNoReviewsForGivenUser()
        {
                List<ReviewedArtPiece> reviews = await _command.ExecuteAsync(new ReviewerId(), 10);

                reviews.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturnReviews_WhenTheyExist()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Reviewer reviewer = await CreateReviewerWith20Reviews(artPieceIds);

                List<ReviewedArtPiece> reviews = await _command.ExecuteAsync(reviewer.Id, 10);

                reviews.Should().HaveCount(10);
        }

        [Fact]
        public async Task Execute_ShouldReturnSomeReviews_WhenOffset()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Reviewer reviewer = await CreateReviewerWith20Reviews(artPieceIds);

                List<ReviewedArtPiece> reviews = await _command.ExecuteAsync(reviewer.Id, 10, 17);

                reviews.Should().HaveCount(3);
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenReviewsExistButForADifferentUser()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                await CreateReviewerWith20Reviews(artPieceIds);

                List<ReviewedArtPiece> reviews = await _command.ExecuteAsync(new ReviewerId(), 10, 0);

                reviews.Should().BeEmpty();
        }

}
