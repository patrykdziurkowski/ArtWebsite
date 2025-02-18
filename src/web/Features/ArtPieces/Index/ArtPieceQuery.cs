using web.Data;
using web.Features.Reviewers;

namespace web.Features.ArtPieces.Index;

public class ArtPieceQuery(ApplicationDbContext dbContext)
{
        public ArtPiece? Execute(Guid currentUserId)
        {
                ReviewerId reviewerId = dbContext.Reviewers
                        .First(r => r.UserId == currentUserId).Id;

                List<ArtPieceId> reviewedArtPieces = dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .Select(r => r.ArtPieceId).ToList();

                return dbContext.ArtPieces
                        .OrderByDescending(a => a.UploadDate)
                        .Where(a => reviewedArtPieces.Contains(a.Id) == false)
                        .FirstOrDefault();
        }

}
