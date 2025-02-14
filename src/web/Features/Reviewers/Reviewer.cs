using FluentResults;
using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class Reviewer : AggreggateRoot
{
        public ReviewerId Id { get; init; } = new ReviewerId();
        public required string Name { get; init; }
        public DateTimeOffset JoinDate { get; init; } = DateTimeOffset.UtcNow;
        public int ReviewCount { get; init; } = 0;
        private List<Like> _likes = [];
        public IEnumerable<Like> Likes
        {
                get { return _likes; }
                init { _likes = value.ToList(); }
        }
        public required Guid UserId { get; init; }

        public Result LikeArtPiece(ArtPieceId artPieceId)
        {
                const int MAX_NUMBER_OF_ACTIVE_LIKES = 5;
                int numberOfActiveLikes = _likes.Where(l => l.IsActive).Count();
                if (numberOfActiveLikes >= MAX_NUMBER_OF_ACTIVE_LIKES)
                {
                        return Result.Fail("Too many active likes.");
                }

                _likes.Add(new Like
                {
                        ArtPieceId = artPieceId
                });
                return Result.Ok();
        }
}
