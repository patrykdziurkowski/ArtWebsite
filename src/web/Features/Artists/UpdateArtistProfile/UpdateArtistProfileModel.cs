using System.ComponentModel.DataAnnotations;
using web.Features.Images;

namespace web.Features.Artists.UpdateArtistProfile;

public class UpdateArtistProfileModel
{
        [Required]
        public required Guid ArtistId { get; init; }

        [Required]
        [StringLength(12, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public required string Name { get; init; }

        [Required]
        public required string Summary { get; init; }

        [DataType(DataType.Upload)]
        [SupportedImage]
        public IFormFile? Image { get; set; } = null;
}
