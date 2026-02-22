using Microsoft.AspNetCore.Identity;
using web.Data;
using web.Features.Artists;

namespace web.Features.ArtPieces;

public class ArtPieceDetailsQuery(
        ArtPieceRepository artPieceRepository,
        ArtistRepository artistRepository,
        UserManager<IdentityUser<Guid>> userManager,
        ApplicationDbContext dbContext)
{
        public async Task<ArtPieceDto> ExecuteAsync(Guid currentUserId, ArtPieceId artPieceId)
        {
                IdentityUser<Guid> currentUser = dbContext.Users.First(u => u.Id == currentUserId);

                ArtPiece artPiece = (await artPieceRepository.GetByIdAsync(artPieceId))!;

                Artist artist = (await artistRepository.GetByIdAsync(artPiece.ArtistId))!;

                return new ArtPieceDto
                {
                        Id = artPiece.Id,
                        ImagePath = artPiece.ImagePath,
                        Description = artPiece.Description,
                        AverageRating = artPiece.AverageRating,
                        UploadDate = artPiece.UploadDate,
                        ArtistId = artPiece.ArtistId,
                        ProfilePicturePath = artist.ProfilePicturePath,
                        LikeCount = artPiece.LikeCount,
                        ReviewCount = artPiece.ReviewCount,
                        ArtistName = artist.Name,
                        ArtistUserId = artist.UserId,
                        CurrentUserIsOwner = artist.UserId == currentUserId,
                        CurrentUserIsAdmin = await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE),
                };
        }
}
