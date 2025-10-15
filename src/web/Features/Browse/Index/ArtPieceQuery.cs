using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Browse.Index;

public class ArtPieceQuery(
        ApplicationDbContext dbContext,
        IMapper mapper)
{
        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId)
        {
                ReviewerId reviewerId = (await dbContext.Reviewers
                        .FirstAsync(r => r.UserId == currentUserId)).Id;

                List<ArtPieceId> reviewedArtPieces = await dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .Select(r => r.ArtPieceId)
                        .ToListAsync();

                ArtPiece? artPiece = await dbContext.ArtPieces
                        .OrderByDescending(ap => dbContext.Boosts.Any(b => b.ArtPieceId == ap.Id))
                        .ThenByDescending(ap => ap.UploadDate)
                        .Where(ap => reviewedArtPieces.Contains(ap.Id) == false)
                        .FirstOrDefaultAsync();

                if (artPiece is null)
                {
                        return null;
                }

                ArtPieceDto artPieceDto = mapper.Map<ArtPieceDto>(artPiece);
                artPieceDto.IsLikedByCurrentUser = await dbContext.Likes
                        .AnyAsync(l => l.ReviewerId == reviewerId && l.ArtPieceId == artPiece.Id);
                return artPieceDto;
        }

}
