using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Browse;

public class RegisterArtPieceServedCommand(ApplicationDbContext dbContext)
{
        public async Task ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId, DateTimeOffset? date = null)
        {
                date ??= DateTimeOffset.UtcNow;

                ArtPieceServed? artPieceServed = await dbContext.ArtPiecesServed
                        .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);
                if (artPieceServed is null)
                {
                        artPieceServed = new()
                        {
                                UserId = currentUserId,
                                ArtPieceId = artPieceId,
                        };
                        await dbContext.AddAsync(artPieceServed);
                }
                else
                {
                        artPieceServed.Date = date.Value;
                        artPieceServed.ArtPieceId = artPieceId;
                }

                await dbContext.SaveChangesAsync();
        }
}
