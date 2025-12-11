namespace web.Features.Reviewers;

public class ReviewerQuery(ReviewerRepository reviewerRepository)
{
        public async Task<Reviewer?> ExecuteAsync(string reviewerName)
        {
                return await reviewerRepository.GetByNameAsync(reviewerName);
        }

}
