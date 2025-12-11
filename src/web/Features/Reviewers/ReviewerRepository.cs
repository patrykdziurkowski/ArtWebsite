using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers.UnlikeArtPiece;
using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class ReviewerRepository(ApplicationDbContext dbContext)
{
        public async Task<Reviewer?> GetByIdAsync(Guid userId)
        {
                Reviewer? reviewer = await dbContext.Reviewers
                        .FirstOrDefaultAsync(r => r.UserId == userId);
                if (reviewer is null)
                {
                        return null;
                }

                return await GetByIdAsync(reviewer.Id);
        }

        public async Task<Reviewer?> GetByIdAsync(ReviewerId reviewerId)
        {
                Reviewer? reviewer = await dbContext.Reviewers
                        .FirstOrDefaultAsync(reviewer => reviewer.Id == reviewerId);
                if (reviewer is null)
                {
                        return null;
                }

                reviewer.ReviewCount = dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewer.Id)
                        .Count();
                reviewer.ActiveLikes = await dbContext.Likes
                        .Where(l => l.ExpirationDate >= DateTimeOffset.UtcNow
                                && l.ReviewerId == reviewer.Id)
                        .ToListAsync();
                return reviewer;
        }

        public async Task<Reviewer?> GetByNameAsync(string name)
        {
                Reviewer? reviewer = await dbContext.Reviewers
                        .FirstOrDefaultAsync(reviewer => reviewer.Name == name);
                if (reviewer is null)
                {
                        return null;
                }

                reviewer.ReviewCount = dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewer.Id)
                        .Count();
                reviewer.ActiveLikes = await dbContext.Likes
                        .Where(l => l.ExpirationDate >= DateTimeOffset.UtcNow
                                && l.ReviewerId == reviewer.Id)
                        .ToListAsync();
                return reviewer;
        }

        public async Task<List<Like>> GetLikesAsync(Guid currentUserId,
                int count, int offset = 0)
        {
                ReviewerId reviewerId = dbContext.Reviewers
                                       .First(reviewer => reviewer.UserId == currentUserId).Id;
                return await dbContext.Likes
                        .Where(like => like.ReviewerId == reviewerId)
                        .OrderByDescending(like => like.Date)
                        .Skip(offset)
                        .Take(count)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task SaveAsync(Reviewer reviewer)
        {
                foreach (IDomainEvent domainEvent in reviewer.DomainEvents)
                {
                        if (domainEvent is ArtPieceUnlikedEvent unlikedEvent)
                        {
                                dbContext.Likes.Remove(unlikedEvent.Like);
                        }
                }

                reviewer.ClearDomainEvents();
                await dbContext.SaveChangesAsync();
        }

}
