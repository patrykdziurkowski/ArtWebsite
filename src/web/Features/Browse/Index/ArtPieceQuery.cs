using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Browse.Index;

public class ArtPieceQuery(
        ArtPieceRepository artPieceRepository,
        ApplicationDbContext dbContext,
        IMapper mapper)
{
        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId)
        {
                ReviewerId reviewerId = await dbContext.Reviewers
                        .Where(r => r.UserId == currentUserId)
                        .Select(r => r.Id)
                        .FirstAsync();

                ArtPiece? artPiece = await artPieceRepository.GetByAlgorithmAsync(reviewerId);
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
