using Microsoft.AspNetCore.Identity;

namespace web.Features.Reviewers.Index;

public class UserReviewerQuery(
        ReviewerRepository reviewerRepository,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task<ReviewerProfileDto> ExecuteAsync(Guid currentUserId)
        {
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                Reviewer reviewer = await reviewerRepository.GetByIdAsync(currentUserId)
                        ?? throw new InvalidOperationException("No reviewer profile found for user.");

                return new ReviewerProfileDto()
                {
                        Id = reviewer.Id,
                        Name = reviewer.Name,
                        JoinDate = reviewer.JoinDate,
                        ReviewCount = reviewer.ReviewCount,
                        Points = reviewer.Points,
                        IsCurrentUserAdmin = await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE),
                        CurrentUserOwnsThisProfile = reviewer.UserId == currentUserId
                };
        }

}
