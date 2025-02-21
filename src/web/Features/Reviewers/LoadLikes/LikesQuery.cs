namespace web.Features.Reviewers.LoadLikes;

public class LikesQuery(ReviewerRepository reviewerRepository)
{
        public async Task<List<Like>> ExecuteAsync(Guid currentUserId,
                int count, int offset = 0)
        {
                return await reviewerRepository
                        .GetLikesAsync(currentUserId, count, offset);
        }
}
