using System.ComponentModel.DataAnnotations;

namespace web.Features.Reviews.ReviewArtPiece;

public class ReviewArtPieceModel
{
        [Required(ErrorMessage = "Please enter a comment.")]
        [Length(50, 500, ErrorMessage = "Please enter a comment between 100 and 500 characters.")]
        public required string Comment { get; set; }
        [Required]
        public required Guid ArtPieceId { get; set; }
}
