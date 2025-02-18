using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces.LoadArtPieces;

public class ArtPiecesQuery(ApplicationDbContext dbContext)
{
        public List<ArtPiece> Execute(ArtistId artistId, int number, int offset = 0)
        {
                return dbContext.ArtPieces.Where(a => a.ArtistId == artistId)
                        .OrderByDescending(a => a.UploadDate)
                        .Skip(offset)
                        .Take(number)
                        .ToList();
        }
}
