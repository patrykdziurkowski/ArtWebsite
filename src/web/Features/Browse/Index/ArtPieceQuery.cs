using AutoMapper;
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
        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId)
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

                ArtPiece? artPiece = await artPieceRepository.GetByAlgorithmAsync(reviewerId);
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
                        ArtistName = artistOwner.Name,
                        ArtistUserId = artistOwner.UserId,
                };
        }

}
