using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Tags;

public class ArtPieceTagsQuery(ApplicationDbContext dbContext)
{
        public async Task<List<Tag>> ExecuteAsync(ArtPieceId artPieceId)
        {
                List<TagId> tagIds = await dbContext.ArtPieceTags.Where(apt => apt.ArtPieceId == artPieceId)
                        .Select(apt => apt.TagId).ToListAsync();
                return await dbContext.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
        }
}
