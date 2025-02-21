using web.Data;

namespace web.Features.Reviewers.Index;

public class UserReviewerQuery(ApplicationDbContext dbContext)
{
        public Reviewer Execute(Guid userId)
        {
                Reviewer r = dbContext.Reviewers
                        .First(reviewer => reviewer.UserId == userId);
                return new Reviewer
                {
                        Id = r.Id,
                        Name = r.Name,
                        JoinDate = r.JoinDate,
                        ReviewCount = dbContext.Reviews
                                .Select(review => review.ReviewerId)
                                .Where(reviewerId => reviewerId == r.Id)
                                .Count(),
                        UserId = r.UserId,
                        ActiveLikes = dbContext.Likes
                                .Where(l => l.ExpirationDate >= DateTimeOffset.UtcNow && l.ReviewerId == r.Id),
                };
        }

}
