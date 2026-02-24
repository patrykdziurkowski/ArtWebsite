using web.Features.Reviews.ReviewArtPiece;

namespace web.Features.Browse;

public record BrowseModel
{
    public required int ArtPiecesLikedToday { get; init; }
    public required int CurrentReviewerActivePoints { get; init; }
    public ReviewArtPieceModel? ReviewForm { get; set; } = null;
}
