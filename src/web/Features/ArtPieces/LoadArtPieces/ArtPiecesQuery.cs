using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces.LoadArtPieces;

public class ArtPiecesQuery(ApplicationDbContext dbContext)
{
        public async Task<List<ArtPiece>> ExecuteAsync(ArtistId artistId, int number, int offset = 0)
        {
                List<ArtPiece> artPieces = await dbContext.ArtPieces
                        .Where(a => a.ArtistId == artistId)
                        .OrderByDescending(a => a.UploadDate)
                        .Skip(offset)
                        .Take(number)
                        .ToListAsync();
                await AssignAverageRatings(artPieces);

                return artPieces;
        }

        private async Task AssignAverageRatings(List<ArtPiece> artPieces)
        {
                foreach (ArtPiece artPiece in artPieces)
                {
                        List<int> ratings = await dbContext.Reviews
                                .Where(r => r.ArtPieceId == artPiece.Id)
                                .Select(r => r.Rating.Value)
                                .ToListAsync();
                        if (ratings.Count == 0)
                        {
                                continue;
                        }
                        artPiece.AverageRating = (int)double.Round(ratings.Average());
                }
        }
}
