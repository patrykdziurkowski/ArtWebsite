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
                        _activeLikes = [.. value];
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

        public Result UnlikeArtPiece(ArtPieceId artPieceId)
        {
                if (!_activeLikes.Any(l => l.ArtPieceId == artPieceId))
                {
                        return Result.Fail("Couldn't unlike the given art piece. No active like found for it.");
                }

                Like like = _activeLikes.Single(l => l.ArtPieceId == artPieceId);
                if (!like.IsActive)
                {
                        return Result.Fail("Couldn't undo an expired like.");
                }

                bool likeTooOld = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(15)) > like.Date;
                if (likeTooOld)
                {
                        return Result.Fail("Couldn't undo a like because too much time has passed.");
                }

                _activeLikes.Remove(like);
                return Result.Ok();
        }
}
