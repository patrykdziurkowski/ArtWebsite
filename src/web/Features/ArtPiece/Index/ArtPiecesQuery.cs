using System;
using Microsoft.EntityFrameworkCore;
using web.data;

namespace web.Features.ArtPiece.Index;

public class ArtPiecesQuery
{
        private readonly ApplicationDbContext _dbContext;

        public ArtPiecesQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }
        public async Task<List<ArtPiece>> ExecuteAsync(int amount, DateTime lastArtPieceUploadDate)
        {
                return _dbContext.ArtPieces
                        .Where(a => a.UploadDate > lastArtPieceUploadDate)
                        .OrderBy(a => a.UploadDate)
                        .Take(amount)
                        .ToList();
        }

        public async Task<List<ArtPiece>> ExecuteAsync(int amount)
        {
                return await ExecuteAsync(amount, DateTime.MinValue);
        }
}
