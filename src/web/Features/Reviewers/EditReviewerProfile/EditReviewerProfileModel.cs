using System.ComponentModel.DataAnnotations;

namespace web.Features.Reviewers.EditReviewerProfile;

public record EditReviewerProfileModel
{
        [Required]
        [StringLength(12, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public required string Name { get; init; }
}
