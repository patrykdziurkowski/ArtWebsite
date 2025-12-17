using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Browse.ByTag;

public class ArtPieceByTagQuery(
        ArtPieceRepository artPieceRepository,
        ApplicationDbContext dbContext,
        IMapper mapper)
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

                return mapper.Map<ArtPieceDto>(artPiece);
        }

}
