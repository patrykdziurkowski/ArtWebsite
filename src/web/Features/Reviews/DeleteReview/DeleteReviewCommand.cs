using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.Reviews.DeleteReview;

public class DeleteReviewCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task ExecuteAsync(Guid currentUserId, ReviewId reviewId)
        {
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                Review review = await dbContext.Reviews.FirstAsync(r => r.Id == reviewId);
                Reviewer reviewOwner = await dbContext.Reviewers.FirstAsync(r => review.ReviewerId == r.Id);

                if (reviewOwner.UserId != currentUserId
                        && await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                {
                        throw new InvalidOperationException("Attempted to delete a review that this user does not own.");
                }

                var test = dbContext.Likes.ToList();
                Like? reviewsLike = await dbContext.Likes.FirstOrDefaultAsync(l => l.ReviewerId == reviewOwner.Id
                        && l.ArtPieceId == review.ArtPieceId);

                if (reviewsLike is not null)
                {
                        dbContext.Remove(reviewsLike);
                }

                dbContext.Remove(review);
                await dbContext.SaveChangesAsync();
        }
}
