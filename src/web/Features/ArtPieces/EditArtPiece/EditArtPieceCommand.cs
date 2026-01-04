using Microsoft.AspNetCore.Identity;
using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces.EditArtPiece;

public class EditArtPieceCommand(
        ArtPieceRepository artPieceRepository,
        ArtistRepository artistRepository,
        UserManager<IdentityUser<Guid>> userManager,
        ApplicationDbContext dbContext)
{
        public async Task ExecuteAsync(
                Guid currentUserId,
                ArtPieceId artPieceId,
                string description)
        {
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                ArtPiece artPiece = (await artPieceRepository.GetByIdAsync(artPieceId))!;
                Artist artist = (await artistRepository.GetByIdAsync(artPiece.ArtistId))!;

                if (artist.UserId != currentUserId)
                {
                        if (await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                        {
                                throw new InvalidOperationException("Unable to edit art piece: current user doesn't own this art piece.");
                        }
                }

                artPiece.Description = description;
                await dbContext.SaveChangesAsync();
        }
}
