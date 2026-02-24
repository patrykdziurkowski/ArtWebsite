using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Browse;

public class RegisterArtPieceServedCommand(ApplicationDbContext dbContext)
{
        public async Task ExecuteAsync(Guid currentUserId, ArtPieceId? artPieceId, DateTimeOffset? date = null)
        {
                date ??= DateTimeOffset.UtcNow;

                ArtPieceServed? artPieceServed = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);

                // doesnt exist but needs to be updated
                if (artPieceServed is null && artPieceId is not null)
                {
                        artPieceServed = new()
                        {
                                UserId = currentUserId,
                                ArtPieceId = artPieceId,
                                Date = date.Value,
                        };
                        await dbContext.AddAsync(artPieceServed);
                }
                // exists but needs to be deleted
                else if (artPieceServed is not null && artPieceId is null)
                {
                        dbContext.Remove(artPieceServed);
                }
                // exists and needs to be updated
                else if (artPieceServed is not null && artPieceId is not null)
                {
                        artPieceServed.Date = date.Value;
                        artPieceServed.ArtPieceId = artPieceId;
                }

                await dbContext.SaveChangesAsync();
        }
}
