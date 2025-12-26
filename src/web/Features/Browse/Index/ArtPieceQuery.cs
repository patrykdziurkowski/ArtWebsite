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
                ArtPieceServed? artPieceServed = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);
                if (artPieceServed is not null)
                {
                        ArtPiece? lastArtPiece = await artPieceRepository.GetByIdAsync(artPieceServed.ArtPieceId);
                        if (lastArtPiece is not null)
                        {
                                return mapper.Map<ArtPieceDto>(lastArtPiece);
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

                return mapper.Map<ArtPieceDto>(artPiece);
        }

}
