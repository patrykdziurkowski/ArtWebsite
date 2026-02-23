using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;

namespace web.Features.Tags.UnassignTag;

public class UnassignTagCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId, string tagToUnassign)
        {
                IdentityUser<Guid> currentUser = await dbContext.Users.FirstAsync(u => u.Id == currentUserId);
                Artist? currentUsersArtist = await dbContext.Artists.FirstOrDefaultAsync(a => a.UserId == currentUserId);
                ArtPiece artPiece = await dbContext.ArtPieces.FirstAsync(ap => ap.Id == artPieceId);

                if ((currentUsersArtist is null || artPiece.ArtistId != currentUsersArtist.Id)
                        && await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                {
                        throw new InvalidOperationException("Unable to unassign a tag from an art piece - current user does not own the art piece.");
                }

                TagId tagToUnassignId = await dbContext.Tags
                        .Where(t => t.Name == tagToUnassign)
                        .Select(t => t.Id)
                        .FirstAsync();
                ArtPieceTag artPieceTagToRemove = await dbContext.ArtPieceTags
                        .FirstAsync(apt => apt.ArtPieceId == artPieceId && apt.TagId == tagToUnassignId);
                dbContext.ArtPieceTags.Remove(artPieceTagToRemove);

                int artPiecesWithThisTag = await dbContext.ArtPieceTags.CountAsync(apt => apt.TagId == tagToUnassignId);
                if (artPiecesWithThisTag == 1)
                {
                        Tag tag = await dbContext.Tags.FirstAsync(t =>  t.Id == tagToUnassignId);
                        dbContext.Tags.Remove(tag);
                }

                await dbContext.SaveChangesAsync();
        }
}
