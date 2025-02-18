using web.Data;
using web.Features.Reviewers;

namespace web.Features.Likes.LoadLikes;

public class LikesQuery(ApplicationDbContext dbContext)
{
        public List<Like> Execute(Guid currentUserId,
                int count, int offset = 0)
        {
                ReviewerId reviewerId = dbContext.Reviewers
                        .First(reviewer => reviewer.UserId == currentUserId).Id;
                return dbContext.Likes
                        .Where(like => like.ReviewerId == reviewerId)
                        .OrderByDescending(like => like.Date)
                        .Skip(offset)
                        .Take(count)
                        .ToList();
        }
}
