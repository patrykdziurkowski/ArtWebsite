using web.Data;

namespace web.Features.Reviewers.Index;

public class UserReviewerQuery
{
        private readonly ApplicationDbContext _dbContext;

        public UserReviewerQuery(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public ReviewerId Execute(Guid userId)
        {
                return _dbContext.Reviewers.First(r => r.UserId == userId).Id;
        }
}
