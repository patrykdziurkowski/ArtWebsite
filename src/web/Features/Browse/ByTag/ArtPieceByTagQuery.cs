using AutoMapper;
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

        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId, string? tagName = null)
        {
                ReviewerId reviewerId = await dbContext.Reviewers
                        .Where(r => r.UserId == currentUserId)
                        .Select(r => r.Id)
                        .FirstAsync();

                ArtPiece? artPiece = await artPieceRepository.GetByAlgorithmAsync(reviewerId, tagName);
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
                        LikeCount = artPiece.LikeCount,
                        ArtistName = artistOwner.Name,
                        ArtistUserId = artistOwner.UserId,
                };
        }

}
