using System.ComponentModel.DataAnnotations;

namespace web.Features.ArtPieces.EditArtPiece;

public record EditArtPieceModel
{
        [Required(ErrorMessage = "Please provide a description.")]
        [MaxLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public required string Description { get; set; }
}
