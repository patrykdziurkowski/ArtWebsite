using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviewers.LikeArtPiece;

namespace tests.Integration;

public class ReviewerRepositoryTests : DatabaseBase
{
        private readonly ReviewerRepository _reviewerRepository;
        private readonly LikeArtPieceCommand _likeArtPieceCommand;

        public ReviewerRepositoryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _reviewerRepository = Scope.ServiceProvider.GetRequiredService<ReviewerRepository>();
                _likeArtPieceCommand = Scope.ServiceProvider.GetRequiredService<LikeArtPieceCommand>();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsReviewer_WhenGivenGuid()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Reviewer reviewer = await CreateReviewerWith20Reviews(artPieceIds);
                await _likeArtPieceCommand.ExecuteAsync(reviewer.UserId, artPieceIds.First());

                Reviewer? queriedReviewer = await _reviewerRepository.GetByIdAsync(reviewer.UserId);

                queriedReviewer!.ReviewCount.Should().Be(20);
                queriedReviewer!.ActiveLikes.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsReviewer_WhenGivenReviewerId()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Reviewer reviewer = await CreateReviewerWith20Reviews(artPieceIds);
                await _likeArtPieceCommand.ExecuteAsync(reviewer.UserId, artPieceIds.First());

                Reviewer? queriedReviewer = await _reviewerRepository.GetByIdAsync(reviewer.Id);

                queriedReviewer!.ReviewCount.Should().Be(20);
                queriedReviewer!.ActiveLikes.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenGivenNonExistantId()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Reviewer reviewer = await CreateReviewerWith20Reviews(artPieceIds);
                await _likeArtPieceCommand.ExecuteAsync(reviewer.UserId, artPieceIds.First());

                Reviewer? queriedReviewer = await _reviewerRepository.GetByIdAsync(new ReviewerId());

                queriedReviewer.Should().BeNull();
        }

        [Fact]
        public async Task SaveAsync_SavesChanges_WhenCalled()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Reviewer reviewer = await CreateReviewerWith20Reviews(artPieceIds);
                await _likeArtPieceCommand.ExecuteAsync(reviewer.UserId, artPieceIds.First());

                reviewer.LikeArtPiece(artPieceIds.Last());
                await _reviewerRepository.SaveAsync(reviewer);

                Reviewer? queriedReviewer = await _reviewerRepository.GetByIdAsync(reviewer.Id);
                queriedReviewer!.ActiveLikes.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetLikesAsync_ShouldReturnEmpty_WhenNoLikesForGivenReviewer()
        {
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;

                List<Like> likes = await _reviewerRepository
                        .GetLikesAsync(currentUserId, 10);

                likes.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLikesAsync_ShouldReturnLikes_WhenTheyExist()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = await CreateReviewerWithLikes(artPieceIds);

                List<Like> likes = await _reviewerRepository
                        .GetLikesAsync(currentUserId, 10);

                likes.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetLikesAsync_ShouldReturnSomeLikes_WhenOffset()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = await CreateReviewerWithLikes(artPieceIds);

                List<Like> likes = await _reviewerRepository
                        .GetLikesAsync(currentUserId, 10, 17);

                likes.Should().HaveCount(3);
        }



}
