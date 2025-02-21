using FluentAssertions;
using FluentResults;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace tests.Unit;

public class ReviewerTests
{
        [Fact]
        public void LikeArtPiece_DoesntAddLike_WhenLessThan5Likes()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        ],
                };
                ArtPieceId artPieceToLikeId = new();

                Result likeResult = reviewer.LikeArtPiece(artPieceToLikeId);

                likeResult.IsSuccess.Should().BeTrue();
                reviewer.ActiveLikes.Should().HaveCount(5);
                reviewer.ActiveLikes.Should()
                        .Contain(like => like.ArtPieceId == artPieceToLikeId);
        }

        [Fact]
        public void LikeArtPiece_AddsLike_When5ActiveLikes()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeReviewer1",
                        UserId = Guid.NewGuid(),
                        ActiveLikes = [
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                                new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        ],
                };
                ArtPieceId artPieceToLikeId = new();

                Result likeResult = reviewer.LikeArtPiece(artPieceToLikeId);

                likeResult.IsSuccess.Should().BeFalse();
                reviewer.ActiveLikes.Should().HaveCount(5);
                reviewer.ActiveLikes.Should()
                        .NotContain(like => like.ArtPieceId == artPieceToLikeId);
        }

}
