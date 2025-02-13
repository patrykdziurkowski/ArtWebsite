using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces.LoadArtPieces;

public class ArtPiecesQuery
{
        private readonly ApplicationDbContext _dbContext;

        public ArtPiecesQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public List<ArtPiece> Execute(ArtistId artistId, int number, int offset = 0)
        {
                return _dbContext.ArtPieces.Where(a => a.ArtistId == artistId)
                        .OrderByDescending(a => a.UploadDate)
                        .Skip(offset)
                        .Take(number)
                        .ToList();
        }
}
