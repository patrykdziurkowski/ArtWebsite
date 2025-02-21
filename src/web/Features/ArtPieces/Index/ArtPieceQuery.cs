using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.ArtPieces.Index;

public class ArtPieceQuery(ApplicationDbContext dbContext)
{
        public async Task<ArtPiece?> ExecuteAsync(Guid currentUserId)
        {
                ReviewerId reviewerId = (await dbContext.Reviewers
                        .FirstAsync(r => r.UserId == currentUserId)).Id;

                List<ArtPieceId> reviewedArtPieces = await dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .Select(r => r.ArtPieceId)
                        .ToListAsync();

                return await dbContext.ArtPieces
                        .OrderByDescending(a => a.UploadDate)
                        .Where(a => reviewedArtPieces.Contains(a.Id) == false)
                        .FirstOrDefaultAsync();
        }

}
