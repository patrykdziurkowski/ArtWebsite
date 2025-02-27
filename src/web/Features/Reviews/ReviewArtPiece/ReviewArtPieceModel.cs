using System.ComponentModel.DataAnnotations;

namespace web.Features.Reviews.ReviewArtPiece;

public class ReviewArtPieceModel
{
        [Required]
        [Length(50, 500, ErrorMessage = "Please enter a comment between {2} and {1} characters.")]
        public required string Comment { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between {2} and {1}.")]
        public required int Rating { get; set; }
        [Required]
        public required Guid ArtPieceId { get; set; }
}
