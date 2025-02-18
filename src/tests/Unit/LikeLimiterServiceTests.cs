using FluentAssertions;
using web.Features.ArtPieces;
using web.Features.Likes;
using web.Features.Likes.LikeArtPiece;
using web.Features.Reviewers;

namespace tests.Unit;

public class LikeLimiterServiceTests
{
        private readonly LikeLimiterService _likeLimiter = new();

        [Fact]
        public void DailyLikeLimitReached_ReturnsFalse_WhenLessThan5Likes()
        {
                List<Like> likes = [
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                ];

                bool limitReached = _likeLimiter.DailyLikeLimitReached(likes);

                limitReached.Should().BeFalse();
        }

        [Fact]
        public void DailyLikeLimitReached_ReturnsFalse_WhenLessThan5ActiveLikes()
        {
                List<Like> likes = [
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(),
                                Date = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)) },
                ];

                bool limitReached = _likeLimiter.DailyLikeLimitReached(likes);

                limitReached.Should().BeFalse();
        }

        [Fact]
        public void DailyLikeLimitReached_ReturnsTrue_When5ActiveLikes()
        {
                List<Like> likes = [
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId() },
                        new Like { ArtPieceId = new ArtPieceId(), ReviewerId = new ReviewerId(),
                                Date = DateTimeOffset.Now.Subtract(TimeSpan.FromHours(7)) },
                ];

                bool limitReached = _likeLimiter.DailyLikeLimitReached(likes);

                limitReached.Should().BeTrue();
        }

}
