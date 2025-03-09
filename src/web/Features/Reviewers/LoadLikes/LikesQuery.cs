using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Shared;

namespace web.Features.Reviewers.LoadLikes;

public class LikesQuery(ReviewerRepository reviewerRepository,
        ApplicationDbContext dbContext,
        IMapper mapper)
{
        public async Task<List<ReviewerLikeModel>> ExecuteAsync(Guid currentUserId,
                int count, int offset = 0)
        {
                List<Like> likes = await reviewerRepository.GetLikesAsync(currentUserId, count, offset);
                Dictionary<ArtPieceId, ArtPiece> artPieces = await dbContext.ArtPieces
                    .Where(a => likes.Select(l => l.ArtPieceId).Contains(a.Id))
                    .ToDictionaryAsync(a => a.Id);

                return likes
                    .Select(like => mapper
                        .Map<Like, ArtPiece, ReviewerLikeModel>(like, artPieces[like.ArtPieceId]))
                    .ToList();
        }
}
