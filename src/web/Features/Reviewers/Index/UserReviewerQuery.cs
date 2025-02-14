using web.Data;

namespace web.Features.Reviewers.Index;

public class UserReviewerQuery
{
        private readonly ApplicationDbContext _dbContext;

        public UserReviewerQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public Reviewer Execute(Guid userId)
        {
                return _dbContext.Reviewers
                        .Where(reviewer => reviewer.UserId == userId)
                        .Select(reviewer => new Reviewer
                        {
                                Id = reviewer.Id,
                                Name = reviewer.Name,
                                JoinDate = reviewer.JoinDate,
                                ReviewCount = _dbContext.Reviews
                                        .Select(review => review.ReviewerId)
                                        .Where(reviewerId => reviewerId == reviewer.Id)
                                        .Count(),
                                UserId = reviewer.UserId,
                        })
                        .First();
        }
}
