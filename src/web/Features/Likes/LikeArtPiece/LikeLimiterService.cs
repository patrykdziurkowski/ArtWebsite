namespace web.Features.Likes.LikeArtPiece;

public class LikeLimiterService
{
        private const int DAILY_LIKE_LIMIT = 5;

        public bool DailyLikeLimitReached(List<Like> likes)
        {
                return likes.Where(l => l.IsActive).Count() >= DAILY_LIKE_LIMIT;
        }
}
