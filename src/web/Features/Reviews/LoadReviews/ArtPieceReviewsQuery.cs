using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Reviews.LoadReviews;

public class ArtPieceReviewsQuery(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task<List<ArtPieceReviewDto>> ExecuteAsync(Guid currentUserId,
                ArtPieceId artPieceId, int count, int offset = 0)
        {
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                bool isAdmin = await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE);

                return await dbContext.Reviews
                        .Where(r => r.ArtPieceId == artPieceId)
                        .Join(
                                dbContext.Reviewers,
                                review => review.ReviewerId,
                                reviewer => reviewer.Id,
                                (review, reviewer) => new ArtPieceReviewDto
                                {
                                        ReviewId = review.Id.Value,
                                        ReviewerName = reviewer.Name,
                                        Rating = review.Rating,
                                        Date = review.Date,
                                        Comment = review.Comment,
                                        Points = reviewer.Points,
                                        IsCurrentUser = reviewer.UserId == currentUserId,
                                        IsAdmin = isAdmin,
                                })
                        .OrderByDescending(dto => dto.IsCurrentUser ? 1 : 0)
                        .ThenByDescending(dto => dto.Points)
                        .Skip(offset)
                        .Take(count)
                        .ToListAsync();
        }

}
