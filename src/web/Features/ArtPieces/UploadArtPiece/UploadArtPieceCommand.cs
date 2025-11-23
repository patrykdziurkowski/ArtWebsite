using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.PointAwards.Artist;
using web.Features.Tags;

namespace web.Features.ArtPieces.UploadArtPiece;

public class UploadArtPieceCommand(
        ApplicationDbContext dbContext,
        ArtistRepository artistRepository,
        ImageTaggingQueue imageTaggingQueue,
        IServiceScopeFactory scopeFactory)
{
        private const int POINTS_PER_UPLOAD = 10;

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
                using (FileStream stream = new(imagePath, FileMode.Create, FileAccess.Write))
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

                artist.Points += POINTS_PER_UPLOAD;
                dbContext.ArtistPointAwards.Add(new ArtistPointAward()
                {
                        ArtistId = artist.Id,
                        PointValue = POINTS_PER_UPLOAD,
                });

                await dbContext.ArtPieces.AddAsync(artPiece);
                await dbContext.SaveChangesAsync();

                imageTaggingQueue.Add(artPieceId, imagePath, async (tags) => await AssignTagsAsync(artPiece.Id, tags));

                return artPiece;
        }

        private async Task AssignTagsAsync(ArtPieceId artPieceId, List<string> tagNames)
        {
                using IServiceScope scope = scopeFactory.CreateScope();
                ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                foreach (string tagName in tagNames)
                {
                        Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                                ?? (await context.Tags.AddAsync(new()
                                {
                                        Name = tagName,
                                })).Entity;

                        await context.ArtPieceTags.AddAsync(new()
                        {
                                TagId = tag.Id,
                                ArtPieceId = artPieceId,
                        });
                }
                await context.SaveChangesAsync();
        }

}
