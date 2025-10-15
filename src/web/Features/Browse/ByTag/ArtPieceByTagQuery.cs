
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Tags;

namespace web.Features.Browse.ByTag;

public class ArtPieceByTagQuery(
        ApplicationDbContext dbContext,
        IMapper mapper)
{
        public async Task<ArtPieceDto?> ExecuteAsync(Guid currentUserId, string tagName)
        {
                ReviewerId reviewerId = await dbContext.Reviewers
                        .Where(r => r.UserId == currentUserId)
                        .Select(r => r.Id)
                        .FirstAsync();

                List<ArtPieceId> reviewedArtPieceIds = await dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .Select(r => r.ArtPieceId)
                        .ToListAsync();

                TagId? tagToFilterById = await dbContext.Tags
                        .Where(t => t.Name == tagName)
                        .Select(t => t.Id)
                        .FirstOrDefaultAsync();
                if (tagToFilterById is null)
                {
                        // no such tag exists
                        return null;
                }

                ArtPiece? artPiece = await dbContext.ArtPieces
                        .Where(ap =>
                                dbContext.ArtPieceTags.Any(at =>
                                        at.ArtPieceId == ap.Id
                                        && at.TagId == tagToFilterById))
                        .Where(ap => !reviewedArtPieceIds.Contains(ap.Id))
                        .OrderByDescending(ap => dbContext.Boosts.Any(b => b.ArtPieceId == ap.Id))
                        .ThenByDescending(ap => ap.UploadDate)
                        .FirstOrDefaultAsync();
                if (artPiece is null)
                {
                        return null;
                }

                ArtPieceDto artPieceDto = mapper.Map<ArtPieceDto>(artPiece);
                artPieceDto.IsLikedByCurrentUser = await dbContext.Likes
                        .AnyAsync(l => l.ReviewerId == reviewerId && l.ArtPieceId == artPiece.Id);
                return artPieceDto;
        }
}
