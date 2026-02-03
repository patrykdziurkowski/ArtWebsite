using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Browse.Index;

public class ArtPieceQuery(
        ArtPieceRepository artPieceRepository,
        ArtistRepository artistRepository,
        ApplicationDbContext dbContext)
{
        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId, ArtPieceId? exceptArtPieceId = null)
        {
                ArtPieceServed? artPieceServed = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);
                if (artPieceServed is not null)
                {
                        ArtPiece? lastArtPiece = await artPieceRepository.GetByIdAsync(artPieceServed.ArtPieceId);
                        if (lastArtPiece is not null)
                        {
                                Artist artist = (await artistRepository.GetByIdAsync(lastArtPiece.ArtistId))!;

                                return new ArtPieceDto
                                {
                                        Id = lastArtPiece.Id,
                                        ImagePath = lastArtPiece.ImagePath,
                                        Description = lastArtPiece.Description,
                                        AverageRating = lastArtPiece.AverageRating,
                                        UploadDate = lastArtPiece.UploadDate,
                                        LikeCount = lastArtPiece.LikeCount,
                                        ProfilePicturePath = artist.ProfilePicturePath,
                                        ReviewCount = lastArtPiece.ReviewCount,
                                        ArtistId = lastArtPiece.ArtistId,
                                        ArtistName = artist.Name,
                                        ArtistUserId = artist.UserId,
                                };
                        }
                }

                ReviewerId reviewerId = await dbContext.Reviewers
                        .Where(r => r.UserId == currentUserId)
                        .Select(r => r.Id)
                        .FirstAsync();

                ArtPiece? artPiece = await artPieceRepository.GetByAlgorithmAsync(reviewerId, exceptArtPieceId: exceptArtPieceId);
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
                        LikeCount = artPiece.LikeCount,
                        ReviewCount = artPiece.ReviewCount,
                        ArtistId = artPiece.ArtistId,
                        ProfilePicturePath = artistOwner.ProfilePicturePath,
                        ArtistName = artistOwner.Name,
                        ArtistUserId = artistOwner.UserId,
                };
        }

}
