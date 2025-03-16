using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces.UploadArtPiece;

public class UploadArtPieceCommand(ApplicationDbContext dbContext,
        ArtistRepository artistRepository)
{
        public async Task<ArtPiece> ExecuteAsync(IFormFile image,
                string description, Guid userId)
        {
                Artist artist = await artistRepository.GetByUserIdAsync(userId)
                        ?? throw new InvalidOperationException("Cannot upload an art piece due to user not having an artist profile.");

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
                await dbContext.ArtPieces.AddAsync(artPiece);
                await dbContext.SaveChangesAsync();
                return artPiece;
        }

}
