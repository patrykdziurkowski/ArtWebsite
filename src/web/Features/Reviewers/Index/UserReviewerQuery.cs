using web.Data;

namespace web.Features.Reviewers.Index;

public class UserReviewerQuery(ApplicationDbContext dbContext)
{
        public Reviewer Execute(Guid userId)
        {
                return dbContext.Reviewers
                        .Where(reviewer => reviewer.UserId == userId)
                        .Select(reviewer => new Reviewer
                        {
                                Id = reviewer.Id,
                                Name = reviewer.Name,
                                JoinDate = reviewer.JoinDate,
                                ReviewCount = dbContext.Reviews
                                        .Select(review => review.ReviewerId)
                                        .Where(reviewerId => reviewerId == reviewer.Id)
                                        .Count(),
                                UserId = reviewer.UserId,
                        })
                        .First();
        }
}
