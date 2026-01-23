using FluentResults;
using Microsoft.AspNetCore.Identity;
using web.Data;
using web.Features.Images;

namespace web.Features.Reviewers.EditReviewerProfile;

public class EditReviewerProfileCommand(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager,
        ImageManager imageManager,
        ReviewerRepository reviewerRepository)
{
        public async Task<Result> ExecuteAsync(
                Guid currentUserId,
                ReviewerId reviewerId,
                string newName,
                IFormFile? newProfilePicture = null)
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

                if (newProfilePicture is not null)
                {
                        string absoluteWebImagePath = await imageManager.UpdateReviewerProfilePictureAsync(
                                newProfilePicture,
                                reviewerId
                        );
                        reviewer.ProfilePicturePath = absoluteWebImagePath;
                }

                reviewer.Name = newName;
                await dbContext.SaveChangesAsync();
                return Result.Ok();
        }
}
