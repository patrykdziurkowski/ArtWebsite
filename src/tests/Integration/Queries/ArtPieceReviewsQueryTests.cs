using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviews;
using web.Features.Reviews.LoadReviews;

namespace tests.Integration.Queries;

public class ArtPieceReviewsQueryTests : DatabaseTest
{
        private readonly ArtPieceReviewsQuery _query;

        public ArtPieceReviewsQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ArtPieceReviewsQuery>();
        }

        [Fact]
        public async Task Execute_ShouldReturnReviews_WhenCurrentReviewerReviewedArtPieceAndTheyExistForArtPiece()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId artPieceWithReviewsId = artPieceIds.First();
                await CreateReviewsForArtPiece(artPieceWithReviewsId, count: 30);
                DbContext.Reviews.Add(new Review()
                {
                        Comment = "Some comment long enough to pass through validation. Some comment long enough to pass through validation. Some comment long enough to pass through validation. Some comment long enough to pass through validation.",
                        Rating = new Rating(4),
                        ReviewerId = (await DbContext.Reviewers.SingleAsync(r => r.UserId == currentUserId)).Id,
                        ArtPieceId = artPieceWithReviewsId,
                });
                await DbContext.SaveChangesAsync();

                List<ArtPieceReviewDto> reviews = await _query.ExecuteAsync(artPieceWithReviewsId, count: 10);

                reviews.Should().HaveCount(10);
        }

        [Fact]
        public async Task Execute_ShouldReturnSomeReviews_WhenOffset()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId artPieceWithReviewsId = artPieceIds.First();
                await CreateReviewsForArtPiece(artPieceWithReviewsId, count: 13);
                DbContext.Reviews.Add(new Review()
                {
                        Comment = "Some comment long enough to pass through validation. Some comment long enough to pass through validation. Some comment long enough to pass through validation. Some comment long enough to pass through validation.",
                        Rating = new Rating(4),
                        ReviewerId = (await DbContext.Reviewers.SingleAsync(r => r.UserId == currentUserId)).Id,
                        ArtPieceId = artPieceWithReviewsId,
                });
                await DbContext.SaveChangesAsync();

                List<ArtPieceReviewDto> reviews = await _query.ExecuteAsync(artPieceWithReviewsId, count: 10, offset: 10);

                reviews.Should().HaveCount(4);
        }

        [Fact]
        public async Task Execute_ReturnedReviews_ShouldStartFromReviewerWithHighestPoints()
        {
                int totalReviewsCount = 30;
                int reviewsToFetchCount = 10;
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = (await DbContext.Users.SingleAsync()).Id;
                ArtPieceId artPieceWithReviewsId = artPieceIds.First();
                await CreateReviewsForArtPiece(artPieceWithReviewsId, totalReviewsCount);
                List<Reviewer> reviewers = await DbContext.Reviewers
                        .Where(r => r.Name.StartsWith("user"))
                        .ToListAsync();
                for (int i = 0; i < reviewers.Count; i++)
                {
                        reviewers[i].Points = i * 100;
                }
                DbContext.Reviews.Add(new Review()
                {
                        Comment = "Some comment long enough to pass through validation. Some comment long enough to pass through validation. Some comment long enough to pass through validation. Some comment long enough to pass through validation.",
                        Rating = new Rating(4),
                        ReviewerId = (await DbContext.Reviewers.SingleAsync(r => r.UserId == currentUserId)).Id,
                        ArtPieceId = artPieceWithReviewsId,
                });
                await DbContext.SaveChangesAsync();


                List<ArtPieceReviewDto> reviews = await _query.ExecuteAsync(artPieceWithReviewsId, reviewsToFetchCount);

                for (int i = 0; i < reviewsToFetchCount; i++)
                {
                        reviews[i].Points.Should().Be((totalReviewsCount - i - 1) * 100);
                }
        }
}
