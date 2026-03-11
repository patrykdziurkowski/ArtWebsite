using System.ComponentModel.DataAnnotations;
using web.Features.Images;

namespace web.Features.ArtPieces.UploadArtPiece;

public class UploadArtPieceModel
{
        [Required(ErrorMessage = "Please choose an image.")]
        [DataType(DataType.Upload)]
        [SupportedImage]
        public required IFormFile Image { get; init; }

        [Required(ErrorMessage = "Please provide a description.")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters.")]
        [MaxLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Description cannot be just whitespace.")]
        public required string Description { get; init; }

}
