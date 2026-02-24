using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.Images;
using web.Features.Leaderboard.Artist;
using web.Features.Missions;
using web.Features.Tags;

namespace web.Features.ArtPieces.UploadArtPiece;

public class UploadArtPieceCommand(
        ApplicationDbContext dbContext,
        ArtistRepository artistRepository,
        ImageTaggingQueue imageTaggingQueue,
        MissionManager missionManager,
        ImageManager imageManager,
        IServiceScopeFactory scopeFactory)
{
        private const int POINTS_PER_UPLOAD = 10;

        public async Task<ArtPiece> ExecuteAsync(IFormFile image,
                string description, Guid userId, DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                Artist artist = await artistRepository.GetByUserIdAsync(userId)
                        ?? throw new InvalidOperationException("Cannot upload an art piece due to user not having an artist profile.");

                ArtPieceId artPieceId = new();
                string absoluteFileWebPath = await imageManager.SaveArtPieceImageAsync(image, artist.Id, artPieceId);

                ArtPiece artPiece = new()
                {
                        Id = artPieceId,
                        Description = description,
                        ImagePath = absoluteFileWebPath,
                        ArtistId = artist.Id,
                        UploadDate = now.Value,
                };

                artist.Points += POINTS_PER_UPLOAD;
                dbContext.ArtistPointAwards.Add(new ArtistPointAward()
                {
                        ArtistId = artist.Id,
                        PointValue = POINTS_PER_UPLOAD,
                        DateAwarded = now.Value,
                });

                await dbContext.ArtPieces.AddAsync(artPiece);
                await dbContext.SaveChangesAsync();

                await missionManager.RecordProgressAsync(MissionType.UploadArt, userId, now.Value);

                imageTaggingQueue.Add(
                        artPieceId,
                        Path.GetFullPath(absoluteFileWebPath.TrimStart(Path.DirectorySeparatorChar)),
                        async (tags) => await AssignTagsAsync(artPiece.Id, tags));

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
