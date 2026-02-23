using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;

namespace web.Features.Tags.AssignTag;

public class AssignTagCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId, string assignedTagName)
        {
                IdentityUser<Guid> currentUser = await dbContext.Users.FirstAsync(u => u.Id == currentUserId);
                Artist? currentUsersArtist = await dbContext.Artists.FirstOrDefaultAsync(a => a.UserId == currentUserId);
                ArtPiece artPiece = await dbContext.ArtPieces.FirstAsync(ap => ap.Id == artPieceId);

                if ((currentUsersArtist is null || artPiece.ArtistId != currentUsersArtist.Id)
                        && await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                {
                        throw new InvalidOperationException("Unable to unassign a tag from an art piece - current user does not own the art piece.");
                }

                Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Name == assignedTagName);
                if (tag is null)
                {
                        tag = new()
                        {
                                Name = assignedTagName,
                        };
                        await dbContext.Tags.AddAsync(tag);
                }
                else if (await dbContext.ArtPieceTags.AnyAsync(apt => apt.ArtPieceId == artPieceId && apt.TagId == tag.Id))
                {
                        // this tag is already assigned to this art piece
                        return;
                }

                await dbContext.ArtPieceTags.AddAsync(new ArtPieceTag
                {
                        ArtPieceId = artPieceId,
                        TagId = tag.Id,
                });

                await dbContext.SaveChangesAsync();
        }
}
