using System.ComponentModel.DataAnnotations;

namespace web.Features.Artists.SetupArtist;

public class SetupModel
{
        [Required]
        [StringLength(16, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public required string Name { get; set; }
        [Required]
        public required string Summary { get; set; }
}