using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviewers.UnlikeArtPiece;

namespace tests.Integration.Commands;

public class UnlikeArtPieceCommandTests : DatabaseTest
{
        private readonly UnlikeArtPieceCommand _command;

        public UnlikeArtPieceCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<UnlikeArtPieceCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotUndoLike_WhenLikeTooOld()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceToLike = artPieceIds.First();
                Reviewer reviewer = await CreateReviewerWithReviewsForArtPieces(artPieceIds);
                DbContext.Likes.Add(new(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(16)))
                {
                        ArtPieceId = artPieceToLike,
                        ReviewerId = reviewer.Id,
                        ReviewId = DbContext.Reviews.First().Id,
                });
                await DbContext.SaveChangesAsync();

                Result result = await _command.ExecuteAsync(reviewer.UserId, artPieceToLike);

                result.IsFailed.Should().BeTrue();
                DbContext.Likes.SingleOrDefault(l => l.ArtPieceId == artPieceToLike).Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenNoLikeInTheFirstPlace()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceToLike = artPieceIds.First();
                Reviewer reviewer = await CreateReviewerWithReviewsForArtPieces(artPieceIds);

                Result result = await _command.ExecuteAsync(reviewer.UserId, artPieceToLike);

                result.IsFailed.Should().BeTrue();
                DbContext.Likes.SingleOrDefault(l => l.ArtPieceId == artPieceToLike).Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUndoLike_WhenValid()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceToLike = artPieceIds.First();
                Reviewer reviewer = await CreateReviewerWithReviewsForArtPieces(artPieceIds);
                DbContext.Likes.Add(new(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(14)))
                {
                        ArtPieceId = artPieceToLike,
                        ReviewerId = reviewer.Id,
                        ReviewId = DbContext.Reviews.First().Id,
                });
                await DbContext.SaveChangesAsync();

                Result result = await _command.ExecuteAsync(reviewer.UserId, artPieceToLike);

                result.IsFailed.Should().BeFalse();
                DbContext.Likes.SingleOrDefault(l => l.ArtPieceId == artPieceToLike).Should().BeNull();
        }

}
