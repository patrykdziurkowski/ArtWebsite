using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.ArtPieces.DeleteArtPiece;

public class DeleteArtPieceCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId)
        {
                ArtPiece artPiece = await dbContext.ArtPieces.FirstAsync(ap => ap.Id == artPieceId);
                Guid artPieceOwnerUserId = (await dbContext.Artists.FirstAsync(a => a.Id == artPiece.ArtistId)).UserId;
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                if (currentUserId != artPieceOwnerUserId
                        && !await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE))
                {
                        throw new InvalidOperationException("Cannot delete art piece: current user is not the owner of this art piece.");
                }

                dbContext.ArtPieces.Remove(artPiece);
                await dbContext.SaveChangesAsync();
                File.Delete("." + artPiece.ImagePath);
        }
}
