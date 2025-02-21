using FluentResults;
using web.Features.ArtPieces;
using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class Reviewer : AggreggateRoot
{
        private const int DAILY_LIKE_LIMIT = 5;

        public ReviewerId Id { get; init; } = new ReviewerId();
        public required string Name { get; init; }
        public DateTimeOffset JoinDate { get; init; } = DateTimeOffset.UtcNow;
        public int ReviewCount { get; set; } = 0;
        public required Guid UserId { get; init; }
        private List<Like> _activeLikes = [];
        public IEnumerable<Like> ActiveLikes
        {
                get => _activeLikes;
                set
                {
                        _activeLikes = value.ToList();
                }
        }

        public Result LikeArtPiece(ArtPieceId artPieceId)
        {
                if (_activeLikes.Count >= DAILY_LIKE_LIMIT)
                {
                        return Result.Fail("Cannot add another like. The limit has been reached.");
                }

                _activeLikes.Add(new Like
                {
                        ArtPieceId = artPieceId,
                        ReviewerId = Id,
                });
                return Result.Ok();
        }
}
