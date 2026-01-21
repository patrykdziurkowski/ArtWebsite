using System.ComponentModel.DataAnnotations;

namespace web.Features.ArtPieces.UploadArtPiece;

public class UploadArtPieceModel
{
        [Required(ErrorMessage = "Please choose an image.")]
        [DataType(DataType.Upload)]
        [SupportedImage]
        public required IFormFile Image { get; init; }

        [Required(ErrorMessage = "Please provide a description.")]
        [MaxLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public required string Description { get; init; }

}
