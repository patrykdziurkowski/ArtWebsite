using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces.LoadArtPieces;

public class ArtPiecesQuery(ApplicationDbContext dbContext)
{
        public async Task<List<ArtPiece>> ExecuteAsync(ArtistId artistId, int number, int offset = 0)
        {
                return await dbContext.ArtPieces.Where(a => a.ArtistId == artistId)
                        .OrderByDescending(a => a.UploadDate)
                        .Skip(offset)
                        .Take(number)
                        .ToListAsync();
        }
}
