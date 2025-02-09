using System.ComponentModel.DataAnnotations;

namespace web.Features.Artists.SetupArtist;

public class SetupModel
{
        [Required]
        [StringLength(12, MinimumLength = 3)]
        public required string Name { get; set; }
        [Required]
        public required string Summary { get; set; }
}