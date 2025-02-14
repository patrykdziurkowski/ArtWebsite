using FluentAssertions;
using FluentResults;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace tests.Unit;

public class ReviewerTests
{
        [Fact]
        public void LikeArtPiece_ShouldAddLike_WhenSuccessful()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeUser123",
                        UserId = Guid.NewGuid(),
                };

                Result result = reviewer.LikeArtPiece(new ArtPieceId());

                result.IsSuccess.Should().BeTrue();
                reviewer.Likes.Should().HaveCount(1);
        }

        [Fact]
        public void LikeArtPiece_ShouldReturnFail_WhenMoreThan5LikesIn24Hours()
        {
                Reviewer reviewer = new()
                {
                        Name = "SomeUser123",
                        UserId = Guid.NewGuid(),
                };
                List<Result> results = [];

                for (int i = 0; i < 6; ++i)
                {
                        Result result = reviewer.LikeArtPiece(new ArtPieceId());
                        results.Add(result);
                }

                for (int i = 0; i < 5; ++i)
                {
                        results[i].IsSuccess.Should().BeTrue();
                }
                results[5].IsFailed.Should().BeTrue();
        }

}
