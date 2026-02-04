using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Browse.ByTag;

public class ArtPieceByTagQuery(
        ArtPieceRepository artPieceRepository,
        ArtistRepository artistRepository,
        ApplicationDbContext dbContext)
{

        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId, string tagName)
        {
                ReviewerId reviewerId = await dbContext.Reviewers
                        .Where(r => r.UserId == currentUserId)
                        .Select(r => r.Id)
                        .FirstAsync();

                ArtPieceServed? artPieceServed = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);

                ArtPieceId? exceptArtPieceId = null;
                if (artPieceServed is not null && artPieceServed.WasSkipped)
                {
                        exceptArtPieceId = artPieceServed.ArtPieceId;
                }

                ArtPiece? artPiece = await artPieceRepository.GetByAlgorithmAsync(reviewerId, exceptArtPieceId, tagName);
                if (artPiece is null)
                {
                        return null;
                }

                Artist artistOwner = (await artistRepository.GetByIdAsync(artPiece.ArtistId))!;

                return new ArtPieceDto
                {
                        Id = artPiece.Id,
                        ImagePath = artPiece.ImagePath,
                        Description = artPiece.Description,
                        AverageRating = artPiece.AverageRating,
                        UploadDate = artPiece.UploadDate,
                        ArtistId = artPiece.ArtistId,
                        ProfilePicturePath = artistOwner.ProfilePicturePath,
                        LikeCount = artPiece.LikeCount,
                        ReviewCount = artPiece.ReviewCount,
                        ArtistName = artistOwner.Name,
                        ArtistUserId = artistOwner.UserId,
                };
        }

}
