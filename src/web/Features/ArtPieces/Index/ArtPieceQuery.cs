using web.Data;
using web.Features.Reviewers;

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
                ReviewerId reviewerId = _dbContext.Reviewers
                        .First(r => r.UserId == currentUserId).Id;

                List<ArtPieceId> reviewedArtPieces = _dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .Select(r => r.ArtPieceId).ToList();

                return _dbContext.ArtPieces
                        .OrderByDescending(a => a.UploadDate)
                        .Where(a => reviewedArtPieces.Contains(a.Id) == false)
                        .FirstOrDefault();
        }

}
