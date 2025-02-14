using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.ArtPieces.UploadArtPiece;

public class UploadArtPieceCommand
{
        private readonly ApplicationDbContext _dbContext;

        public UploadArtPieceCommand(
                ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public async Task<ArtPiece> ExecuteAsync(IFormFile image,
                string description, Guid userId)
        {
                Artists.Artist artist = await _dbContext.Artists
                        .FirstAsync(a => a.UserId == userId);

                string directoryPath = Path.Combine("user-images", "art-pieces", $"{artist.Id}");
                Directory.CreateDirectory(directoryPath);

                ArtPieceId artPieceId = new();
                string fileExtension = Path.GetExtension(image.FileName);
                string imagePath = Path.Combine(directoryPath, $"{artPieceId}{fileExtension}");
                using (FileStream stream = new(imagePath, FileMode.Create))
                {
                        await image.CopyToAsync(stream);
                }

                ArtPiece artPiece = new()
                {
                        Id = artPieceId,
                        Description = description,
                        ImagePath = imagePath,
                        ArtistId = artist.Id,
                };
                await _dbContext.ArtPieces.AddAsync(artPiece);
                await _dbContext.SaveChangesAsync();
                return artPiece;
        }

}
