using Microsoft.AspNetCore.Identity;
using web.Features.Reviewers.Index;

namespace web.Features.Reviewers;

public class ReviewerQuery(
        ReviewerRepository reviewerRepository,
        UserManager<IdentityUser<Guid>> userManager)
{
        public async Task<ReviewerProfileDto?> ExecuteAsync(Guid currentUserId, string reviewerName)
        {
                Reviewer? reviewer = await reviewerRepository.GetByNameAsync(reviewerName);
                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                if (reviewer is null)
                {
                        return null;
                }

                return new ReviewerProfileDto()
                {
                        Id = reviewer.Id,
                        ReviewerUserId = reviewer.UserId,
                        Name = reviewer.Name,
                        JoinDate = reviewer.JoinDate,
                        ReviewCount = reviewer.ReviewCount,
                        Points = reviewer.Points,
                        IsCurrentUserAdmin = await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE),
                        CurrentUserOwnsThisProfile = reviewer.UserId == currentUserId
                };
        }

}
