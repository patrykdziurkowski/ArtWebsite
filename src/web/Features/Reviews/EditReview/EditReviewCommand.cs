using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.Reviews.EditReview;

public class EditReviewCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task ExecuteAsync(
                Guid currentUserId,
                ReviewId reviewId,
                Rating newRating,
                string newComment)
        {
                Review review = await dbContext.Reviews.FirstAsync(r => r.Id == reviewId);
                Reviewer reviewer = await dbContext.Reviewers.FirstAsync(r => r.Id == review.ReviewerId);
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;

                if (currentUserId != reviewer.UserId)
                {
                        if (await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                        {
                                throw new InvalidOperationException("Unable to edit review. Current user is not this review's owner.");
                        }
                }

                review.Rating = newRating;
                review.Comment = newComment;
                await dbContext.SaveChangesAsync();
        }
}
