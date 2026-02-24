using FluentAssertions;
using FluentResults;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviews;

namespace tests.Unit;

public class ReviewerTests
{
        [Fact]
        public void LikeArtPiece_AddsLike_WhenLessThan5Likes()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                        ],
                };
                ArtPieceId artPieceToLikeId = new();

                Result likeResult = reviewer.LikeArtPiece(artPieceToLikeId, new ReviewId());

                likeResult.IsSuccess.Should().BeTrue();
                reviewer.ActiveLikes.Should().HaveCount(5);
                reviewer.ActiveLikes.Should()
                        .Contain(like => like.ArtPieceId == artPieceToLikeId);
                reviewer.Points.Should().Be(15);
                reviewer.ActivePoints.Should().Be(15);
        }

        [Fact]
        public void LikeArtPiece_DoesntAddLike_When5ActiveLikes()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                        ],
                };
                ArtPieceId artPieceToLikeId = new();

                Result likeResult = reviewer.LikeArtPiece(artPieceToLikeId, new ReviewId());

                likeResult.IsSuccess.Should().BeFalse();
                reviewer.ActiveLikes.Should().HaveCount(5);
                reviewer.ActiveLikes.Should()
                        .NotContain(like => like.ArtPieceId == artPieceToLikeId);
                reviewer.Points.Should().Be(0);
                reviewer.ActivePoints.Should().Be(0);
        }

        [Fact]
        public void UnlikeArtPiece_ReturnsFail_WhenTryingToUnlikeANonLikedArtPiece()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                        ],
                };

                Result unlikeResult = reviewer.UnlikeArtPiece(new ArtPieceId());

                unlikeResult.IsFailed.Should().BeTrue();
                reviewer.Points.Should().Be(0);
                reviewer.ActivePoints.Should().Be(0);
        }

        [Fact]
        public void UnlikeArtPiece_ReturnsFail_WhenTryingToUnlikeAnArtPieceThatsExpired()
        {
                ArtPieceId artPieceToUnlike = new();
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like(date: DateTimeOffset.MinValue) { ArtPieceId = artPieceToUnlike, ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                        ],
                };

                Result unlikeResult = reviewer.UnlikeArtPiece(artPieceToUnlike);

                unlikeResult.IsFailed.Should().BeTrue();
                reviewer.Points.Should().Be(0);
                reviewer.ActivePoints.Should().Be(0);
        }

        [Fact]
        public void UnlikeArtPiece_ReturnsFail_WhenTryingToUndoALikeThatsOver15MinsLong()
        {
                ArtPieceId artPieceToUnlike = new();
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like(date: DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(16))) { ArtPieceId = artPieceToUnlike, ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                        ],
                };

                Result unlikeResult = reviewer.UnlikeArtPiece(artPieceToUnlike);

                unlikeResult.IsFailed.Should().BeTrue();
                reviewer.Points.Should().Be(0);
                reviewer.ActivePoints.Should().Be(0);
        }

        [Fact]
        public void UnlikeArtPiece_ReturnsOkAndRemovesLike_WhenValid()
        {
                ArtPieceId artPieceToUnlike = new();
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                                new Like(date: DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(14))) { ArtPieceId = artPieceToUnlike, ReviewerId = new ReviewerId(), ReviewId = new ReviewId() },
                        ],
                        Points = 15,
                        ActivePoints = 15,
                };

                Result unlikeResult = reviewer.UnlikeArtPiece(artPieceToUnlike);

                unlikeResult.IsFailed.Should().BeFalse();
                reviewer.ActiveLikes.SingleOrDefault(l => l.ArtPieceId == artPieceToUnlike).Should().BeNull();
                reviewer.Points.Should().Be(0);
                reviewer.ActivePoints.Should().Be(0);
        }

}
