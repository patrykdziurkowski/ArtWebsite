using FluentResults;
using Microsoft.AspNetCore.Identity;
using web.Data;

namespace web.Features.Reviewers.EditReviewerProfile;

public class EditReviewerProfileCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager,
        ReviewerRepository reviewerRepository)
{
        public async Task<Result> ExecuteAsync(Guid currentUserId, ReviewerId reviewerId, string newName)
        {
                Reviewer reviewer = await reviewerRepository.GetByIdAsync(reviewerId)
                        ?? throw new InvalidOperationException("No reviewer with the given id found.");
                if (await reviewerRepository.GetByNameAsync(newName) is not null)
                {
                        return Result.Fail("The given reviewer name is already taken.");
                }

                if (reviewer.UserId != currentUserId)
                {
                        IdentityUser<Guid> currentUser = await userManager.FindByIdAsync(currentUserId.ToString())
                                ?? throw new InvalidOperationException("No user with the given id found.");

                        if (await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                        {
                                throw new InvalidOperationException("Cannot edit this reviewer profile: current user is not its owner.");
                        }
                }

                reviewer.Name = newName;
                await dbContext.SaveChangesAsync();
                return Result.Ok();
        }
}
