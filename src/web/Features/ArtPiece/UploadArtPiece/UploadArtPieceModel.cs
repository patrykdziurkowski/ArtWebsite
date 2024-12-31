using System;
using System.ComponentModel.DataAnnotations;

namespace web.Features.ArtPiece.UploadArtPiece;

public class UploadArtPieceModel
{
        [Required(ErrorMessage = "Please choose an image.")]
        [DataType(DataType.Upload)]
        public required IFormFile Image { get; set; }

        [Required(ErrorMessage = "Please provide a description.")]
        [MaxLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public required string Description { get; set; }

}
