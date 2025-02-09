using web.Data;

namespace web.Features.ArtPieces.Index;

public class ArtPieceQuery
{
        private readonly ApplicationDbContext _dbContext;

        public ArtPieceQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }
        public ArtPiece? Execute()
        {
                return _dbContext.ArtPieces
                        .OrderBy(a => a.UploadDate)
                        .FirstOrDefault();
        }

}
