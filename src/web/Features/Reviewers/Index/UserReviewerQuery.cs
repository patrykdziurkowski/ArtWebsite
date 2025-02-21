namespace web.Features.Reviewers.Index;

public class UserReviewerQuery(ReviewerRepository reviewerRepository)
{
        public async Task<Reviewer?> ExecuteAsync(Guid userId)
        {
                return await reviewerRepository.GetByIdAsync(userId);
        }

}
