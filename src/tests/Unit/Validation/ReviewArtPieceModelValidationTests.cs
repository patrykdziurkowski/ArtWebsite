using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using tests.Unit.Fixtures;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Unit.Validation;

public class ReviewArtPieceModelValidationTests : ValidationBase
{
        [Theory]
        [InlineData("")]
        [InlineData("short comment")]
        [InlineData("This comment is exactly 49 characters and not 50.")]
        [InlineData("This comment is so long it exceeds the 500 character limit. Soooooooooooooooooooooooooooo" +
        "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo" +
        "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo" +
        "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo" +
        "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo" +
        " long...")]
        public void Validate_ShouldFail_WhenCommentInvalid(string comment)
        {
                ReviewArtPieceModel model = new()
                {
                        Comment = comment,
                        Rating = 5,
                        ArtPieceId = Guid.NewGuid(),
                };

                IList<ValidationResult> errors = Validate(model);

                errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(6)]
        public void Validate_ShouldFail_WhenRatingInvalid(int rating)
        {
                ReviewArtPieceModel model = new()
                {
                        Comment = "This comment has a perfectly valid comment length.",
                        Rating = rating,
                        ArtPieceId = Guid.NewGuid(),
                };

                IList<ValidationResult> errors = Validate(model);

                errors.Should().NotBeEmpty();
        }

        [Fact]
        public void Validate_ShouldPass_WhenEverythingIsValid()
        {
                ReviewArtPieceModel model = new()
                {
                        Comment = "This comment has a perfectly valid comment length.",
                        Rating = 5,
                        ArtPieceId = Guid.NewGuid(),
                };

                IList<ValidationResult> errors = Validate(model);

                errors.Should().BeEmpty();
        }

}
