using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using tests.Unit.Fixtures;
using web.Features.Artists.SetupArtist;

namespace tests.Unit.Validation;

public class SetupModelValidationTests : ValidationBase
{
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validation_ShouldFail_WhenSummaryNotProvided(string? summary)
        {
                SetupModel model = new()
                {
                        Name = "SomeUser123",
                        Summary = summary!,
                };

                IList<ValidationResult> validationErrors = Validate(model);

                validationErrors.Should().HaveCount(1);
        }

        [Fact]
        public void Validation_ShouldSucceed_WhenEverythingCorrect()
        {
                SetupModel model = new()
                {
                        Name = "SomeUser123",
                        Summary = "This is a summary of my profile!",
                };

                IList<ValidationResult> validationErrors = Validate(model);

                validationErrors.Should().BeEmpty();
        }
}
