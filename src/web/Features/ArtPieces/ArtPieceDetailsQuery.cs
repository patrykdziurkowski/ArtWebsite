using web.Features.Artists;

namespace web.Features.ArtPieces;

public class ArtPieceDetailsQuery(
        ArtPieceRepository artPieceRepository,
        ArtistRepository artistRepository)
{
        public async Task<ArtPieceDto> ExecuteAsync(ArtPieceId artPieceId)
        {
                ArtPiece artPiece = (await artPieceRepository.GetByIdAsync(artPieceId))!;

                Artist artist = (await artistRepository.GetByIdAsync(artPiece.ArtistId))!;

                return new ArtPieceDto
                {
                        Id = artPiece.Id,
                        ImagePath = artPiece.ImagePath,
                        Description = artPiece.Description,
                        AverageRating = artPiece.AverageRating,
                        UploadDate = artPiece.UploadDate,
                        ArtistId = artPiece.ArtistId,
                        LikeCount = artPiece.LikeCount,
                        ArtistName = artist.Name,
                        ArtistUserId = artist.UserId,
                };
        }
}
