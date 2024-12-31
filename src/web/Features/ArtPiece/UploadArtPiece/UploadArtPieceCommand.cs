using Microsoft.EntityFrameworkCore;
using web.data;
using web.features.artist;

namespace web.Features.ArtPiece.UploadArtPiece;

public class UploadArtPieceCommand
{
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public UploadArtPieceCommand(
                ApplicationDbContext dbContext,
                IWebHostEnvironment env)
        {
                _dbContext = dbContext;
                _env = env;
        }

        public async Task<ArtPiece> ExecuteAsync(IFormFile image,
                string description, string userId)
        {
                Artist artist = await _dbContext.Artists
                        .FirstAsync(a => a.OwnerId == userId);

                string directoryPath = Path.Combine(_env.ContentRootPath, "user-images", "art-pieces", $"{artist.Id}");
                Directory.CreateDirectory(directoryPath);

                ArtPieceId artPieceId = new();
                string imagePath = Path.Combine(directoryPath, $"{artPieceId}");
                using (FileStream stream = new(imagePath, FileMode.Create))
                {
                        await image.CopyToAsync(stream);
                }

                ArtPiece artPiece = new(artPieceId, imagePath,
                        description, DateTime.UtcNow, artist.Id);
                await _dbContext.ArtPieces.AddAsync(artPiece);
                await _dbContext.SaveChangesAsync();
                return artPiece;
        }

}
