using web.Data;
using web.Features.Reviews;

namespace web.Features.ArtPieces.Index;

public class ArtPieceQuery
{
        private readonly ApplicationDbContext _dbContext;

        public ArtPieceQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }
        public ArtPiece? Execute(Guid currentUserId)
        {
                List<ArtPieceId> reviewedArtPieces = _dbContext.Reviews
                        .Where(r => r.ReviewerId == currentUserId)
                        .Select(r => r.ArtPieceId).ToList();

                return _dbContext.ArtPieces
                        .OrderByDescending(a => a.UploadDate)
                        .Where(a => reviewedArtPieces.Contains(a.Id) == false)
                        .FirstOrDefault();
        }

}
